using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;






// şah çekilen konum highlightsiz olmasina rağmen hareket edilebiliyor

namespace ChessByMe
{
    public enum PieceColors
    {
        BLACK,
        WHITE
    }

    public enum Pieces
    {
        PAWN,
        ROOK,
        KNIGHT,
        BISHOP,
        QUEEN,
        KING,
        EMPTY
    }


    public class Square : Panel
    {
        public int lineNumber;
        public int columnNumber;
        public PieceColors PieceColor;
        public Pieces Piece;

        public void FillSquare()
        {
            if (Piece == Pieces.EMPTY) SetImage(this, null);
            else SetImage(this, GetImage(this.Piece, this.PieceColor));
        }

        public static void SetImage(Panel Panel, Image img)
        {
            Panel.BackgroundImage = img;
        }

        public void PlacePiece(Pieces piece, PieceColors color)
        {
            this.Piece = piece;
            this.PieceColor = color;
            FillSquare();
        }



        public static Image GetImage(Pieces piece, PieceColors color)
        {
            string path = "pieces";
            string fileName = string.Empty;
            fileName += (color == PieceColors.BLACK ? "black" : "white");
            fileName += "-";
            fileName +=
                  piece == Pieces.PAWN ? "pawn"
                : piece == Pieces.ROOK ? "rook"
                : piece == Pieces.KNIGHT ? "knight"
                : piece == Pieces.BISHOP ? "bishop"
                : piece == Pieces.QUEEN ? "queen"
                : piece == Pieces.KING ? "king" : string.Empty;
            fileName += ".png";
            Image img = Image.FromFile(path+"/"+fileName);
            return img;
        }
    }


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int squareEdgePixel = 90;
        int formLeftCut = 0;
        int formRightCut = 0;
        int formUpCut = 40;
        int formDownCut = 0;
        int boardVerticalSquareCount = 8;
        int boardHorizontalSquareCount = 8;
        int queue = (int)PieceColors.WHITE;

        Color squareColorLight = Color.White;
        Color squareColorDark = Color.FromArgb(118, 150, 86);
        Color focusColor = Color.FromArgb(245, 246, 130);
        Color highlightColor = Color.Red;

        Form form;
        Square[,] board;



        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr one, int two, int three, int four);

        private void topBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }
        bool isMoveLegal(Square[,] board, Square oldSquare, Square newSquare)
        {
            return (isMovePossible(board, oldSquare, newSquare) && simulateMove(board, oldSquare, newSquare));
        }
        bool isMovePossible(Square[,] board, Square oldSquare, Square newSquare)
        {
            if (oldSquare.Piece == Pieces.EMPTY) return false;
            if (newSquare.Piece != Pieces.EMPTY && newSquare.PieceColor == oldSquare.PieceColor) return false;

            if (oldSquare.Piece == Pieces.PAWN)
            {

                if (oldSquare.PieceColor == PieceColors.WHITE)
                {
                    try
                    {
                        if (board[oldSquare.lineNumber-1, oldSquare.columnNumber+1] == newSquare && newSquare.Piece != Pieces.EMPTY) return true;

                    }
                    catch { }
                    try
                    {
                        if (board[oldSquare.lineNumber-1, oldSquare.columnNumber-1] == newSquare && newSquare.Piece != Pieces.EMPTY) return true;
                    }
                    catch { }
                    if (newSquare.Piece != Pieces.EMPTY) return false;

                    if (oldSquare.lineNumber == 6)
                    {
                        if (newSquare.lineNumber == 5 || newSquare.lineNumber == 4)
                            if (newSquare.columnNumber == oldSquare.columnNumber)
                                return true;
                            else return false;
                        else return false;
                    }

                    else
                    {
                        if (oldSquare.lineNumber-newSquare.lineNumber  == 1)
                            if (newSquare.columnNumber == oldSquare.columnNumber)
                                return true;
                            else return false;
                        else return false;
                    }
                }
                else
                {
                    try
                    {
                        if (board[oldSquare.lineNumber+1, oldSquare.columnNumber+1] == newSquare && newSquare.Piece != Pieces.EMPTY) return true;

                    }
                    catch { }
                    try
                    {
                        if (board[oldSquare.lineNumber+1, oldSquare.columnNumber-1] == newSquare && newSquare.Piece != Pieces.EMPTY) return true;

                    }
                    catch { }
                    if (newSquare.Piece != Pieces.EMPTY) return false;

                    if (oldSquare.lineNumber == 1)
                    {
                        if (newSquare.lineNumber == 2 || newSquare.lineNumber == 3)
                            if (newSquare.columnNumber == oldSquare.columnNumber)
                                return true;
                            else return false;
                        else return false;
                    }

                    else
                    {
                        if (newSquare.lineNumber -oldSquare.lineNumber == 1)
                            if (newSquare.columnNumber == oldSquare.columnNumber)
                                return true;
                            else return false;
                        else return false;
                    }
                }

            }
            if (oldSquare.Piece == Pieces.KNIGHT)
            {
                if (Math.Abs(newSquare.lineNumber - oldSquare.lineNumber) == 2 && Math.Abs(newSquare.columnNumber - oldSquare.columnNumber) == 1)
                    return true;
                if (Math.Abs(newSquare.lineNumber - oldSquare.lineNumber) == 1 && Math.Abs(newSquare.columnNumber - oldSquare.columnNumber) == 2)
                    return true;

                return false;
            }
            if (oldSquare.Piece == Pieces.BISHOP)
            {
                if (Math.Abs(newSquare.lineNumber-oldSquare.lineNumber) == Math.Abs(newSquare.columnNumber - oldSquare.columnNumber))
                {

                    if (isThereAPieceBetween(board, oldSquare, newSquare)) return false;
                    else return true;


                }
                return false;
            }
            if (oldSquare.Piece == Pieces.ROOK)
            {
                if (newSquare.lineNumber == oldSquare.lineNumber || newSquare.columnNumber == oldSquare.columnNumber)
                {
                    if (isThereAPieceBetween(board, oldSquare, newSquare)) return false;
                    else return true;
                }
                return false;
            }
            if (oldSquare.Piece == Pieces.QUEEN)
            {
                if (Math.Abs(newSquare.lineNumber-oldSquare.lineNumber) == Math.Abs(newSquare.columnNumber - oldSquare.columnNumber))
                {
                    if (isThereAPieceBetween(board, oldSquare, newSquare)) return false;
                    else return true;
                }
                if (newSquare.lineNumber == oldSquare.lineNumber || newSquare.columnNumber == oldSquare.columnNumber)
                {
                    if (isThereAPieceBetween(board, oldSquare, newSquare)) return false;
                    else return true;
                }
                return false;
            }
            if (oldSquare.Piece == Pieces.KING)
            {
                if (Math.Abs(newSquare.lineNumber - oldSquare.lineNumber) <= 1 && Math.Abs(newSquare.columnNumber - oldSquare.columnNumber) <= 1) return true;
                else
                {
                    if (newSquare == board[7, 2] && oldSquare.PieceColor == PieceColors.WHITE && !isWhiteKingMoved && !isA1RookMoved && !isThereAPieceBetween(board, oldSquare, board[7, 0]))
                    {
                        return true;
                    }
                    if (newSquare == board[7, 6] && oldSquare.PieceColor == PieceColors.WHITE && !isWhiteKingMoved && !isH1RookMoved && !isThereAPieceBetween(board, oldSquare, board[7, 7]))
                    {
                        return true;
                    }
                    if (newSquare == board[0, 2] && oldSquare.PieceColor == PieceColors.BLACK && !isBlackKingMoved && !isA8RookMoved && !isThereAPieceBetween(board, oldSquare, board[0, 0]))
                    {
                        return true;
                    }
                    if (newSquare == board[0, 6] && oldSquare.PieceColor == PieceColors.BLACK && !isBlackKingMoved && !isH8RookMoved && !isThereAPieceBetween(board, oldSquare, board[0, 7]))
                    {
                        return true;
                    }
                    return false;
                }
            }
            return true;
        }

        //bool isNextTo(Square oldSquare, Square newSquare)
        //{
        //    try { if (oldSquare == board[newSquare.lineNumber+1, newSquare.columnNumber-1]) return true; } catch { }
        //    try { if (oldSquare == board[newSquare.lineNumber+1, newSquare.columnNumber+0]) return true; } catch { }
        //    try { if (oldSquare == board[newSquare.lineNumber+1, newSquare.columnNumber+1]) return true; } catch { }
        //    try { if (oldSquare== board[newSquare.lineNumber+0, newSquare.columnNumber-1]) return true; } catch { }
        //    try { if (oldSquare== board[newSquare.lineNumber+0, newSquare.columnNumber+1]) return true; } catch { }
        //    try { if (oldSquare== board[newSquare.lineNumber-1, newSquare.columnNumber-1]) return true; } catch { }
        //    try { if (oldSquare== board[newSquare.lineNumber-1, newSquare.columnNumber+0]) return true; } catch { }
        //    try { if (oldSquare== board[newSquare.lineNumber-1, newSquare.columnNumber+1]) return true; } catch { }
        //    return false;
        //}
        void HighlightPossibleMoves()
        {
            for (int line = 0; line < board.GetLength(0); line++)
            {
                for (int column = 0; column < board.GetLength(1); column++)
                {
                    if (isMoveLegal(board, CurrentFocusedSquare, board[line, column]))
                    {
                        board[line, column].BackColor = highlightColor;
                    }
                }
            }
        }

        bool isThereAPieceBetween(Square[,] board, Square oldSquare, Square newSquare)
        {
            int lineNumber = oldSquare.lineNumber;
            int columnNumber = oldSquare.columnNumber;

            while (lineNumber != newSquare.lineNumber || columnNumber != newSquare.columnNumber)
            {
                if (lineNumber < newSquare.lineNumber) lineNumber++;
                else if (lineNumber > newSquare.lineNumber) lineNumber--;
                if (columnNumber < newSquare.columnNumber) columnNumber++;
                else if (columnNumber > newSquare.columnNumber) columnNumber--;

                if (board[lineNumber, columnNumber].Piece != Pieces.EMPTY && newSquare != board[lineNumber, columnNumber]) return true;
            }
            return false;
        }

        Square getKing(Square[,] board, PieceColors color)
        {
            for (int line = 0; line < board.GetLength(0); line++)
            {
                for (int column = 0; column < board.GetLength(1); column++)
                {
                    if (board[line, column].Piece == Pieces.KING && board[line, column].PieceColor == color) return board[line, column];
                }
            }
            return null;
        }

        bool ifCheck(Square[,] board, PieceColors color)
        {
            Square king;
            if (color == PieceColors.BLACK) king = getKing(board, PieceColors.BLACK);
            else king = getKing(board, PieceColors.WHITE);

            for (int line = 0; line < board.GetLength(0); line++)
            {
                for (int column = 0; column < board.GetLength(1); column++)
                {
                    if (isMovePossible(board, board[line, column], king)) {  return true; }
                }
            }

            return false;
        }

        bool simulateMove(Square[,] board, Square oldSquare, Square newSquare)
        {
            Square[,] cloneBoard = CloneBoard(board);

            //   castles(cloneBoard,oldSquare, newSquare);


            cloneBoard[newSquare.lineNumber, newSquare.columnNumber].PlacePiece(oldSquare.Piece, oldSquare.PieceColor);
            cloneBoard[oldSquare.lineNumber, oldSquare.columnNumber].PlacePiece(Pieces.EMPTY, PieceColors.WHITE);

            //MessageBox.Show(newSquare.lineNumber+" "+newSquare.columnNumber+" "+ifCheck(cloneBoard, oldSquare.PieceColor));

            return !ifCheck(cloneBoard, oldSquare.PieceColor);
        }
        enum Notation
        {
            A, B, C, D, E, F, G, H
        }
        Square[,] CloneBoard(Square[,] board)
        {
            Square[,] cloneBoard = new Square[board.GetLength(0), board.GetLength(1)];
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int k = 0; k < board.GetLength(1); k++)
                {
                    cloneBoard[i, k] = new Square();
                    cloneBoard[i, k].columnNumber = board[i, k].columnNumber;
                    cloneBoard[i, k].lineNumber = board[i, k].lineNumber;
                    cloneBoard[i, k].Piece = board[i, k].Piece;
                    cloneBoard[i, k].PieceColor = board[i, k].PieceColor;
                }
            }
            return cloneBoard;
        }





        void updateCastlesRequirements(Square oldSquare)
        {
            if (oldSquare.lineNumber == 0 && oldSquare.columnNumber == 0) isA8RookMoved = true;
            if (oldSquare.lineNumber == 7 && oldSquare.columnNumber == 0) isA1RookMoved = true;
            if (oldSquare.lineNumber == 7 && oldSquare.columnNumber == 7) isH1RookMoved = true;
            if (oldSquare.lineNumber == 0 && oldSquare.columnNumber == 7) isH8RookMoved = true;
            if (oldSquare.PieceColor == PieceColors.BLACK && oldSquare.Piece == Pieces.KING) isBlackKingMoved = true;
            if (oldSquare.PieceColor == PieceColors.WHITE && oldSquare.Piece == Pieces.KING) isWhiteKingMoved = true;
        }

        bool Move(Square[,] board, Square oldSquare, Square newSquare)
        {
            if (!isMoveLegal(board, oldSquare, newSquare)) return false;
            if ((int)oldSquare.PieceColor != queue) return false;

            if (oldSquare.Piece == Pieces.EMPTY) return false;

            castles(board, oldSquare, newSquare);

            newSquare.PlacePiece(oldSquare.Piece, oldSquare.PieceColor);
            oldSquare.PlacePiece(Pieces.EMPTY, PieceColors.WHITE);
            //LastFocusedSquare = null;
            //CurrentFocusedSquare = null;

            if (queue == (int)PieceColors.WHITE) queue = (int)PieceColors.BLACK;
            else queue = (int)PieceColors.WHITE;

            updateCastlesRequirements(oldSquare);
            return true;
        }



        void castles(Square[,] board, Square oldSquare, Square newSquare)
        {
            //  MessageBox.Show(oldSquare.Piece+" "+Math.Abs(newSquare.columnNumber-oldSquare.columnNumber));
            if (oldSquare.Piece == Pieces.KING && Math.Abs(newSquare.columnNumber-oldSquare.columnNumber) > 1)
            {

                if (newSquare.columnNumber == 6 && newSquare.lineNumber == 0)
                {
                    Move(board, board[0, 7], board[0, 5]);

                }
                else if (newSquare.columnNumber == 6 && newSquare.lineNumber == 7)
                {
                    Move(board, board[7, 7], board[7, 5]);

                }
                else if (newSquare.columnNumber == 2 && newSquare.lineNumber == 0)
                {
                    Move(board, board[0, 0], board[0, 3]);

                }
                else if (newSquare.columnNumber == 2 && newSquare.lineNumber == 7)
                {
                    Move(board, board[7, 0], board[7, 3]);

                }

                if (queue == (int)PieceColors.WHITE) queue = (int)PieceColors.BLACK;
                else queue = (int)PieceColors.WHITE;
            }
        }

        void NewBackground()
        {
            for (int line = 0; line < board.GetLength(0); line++)
            {
                for (int column = 0; column < board.GetLength(1); column++)
                {
                    board[line, column].BackColor = ((line+column)%2==1) ? squareColorDark : squareColorLight;
                }
            }
        }

        bool isA1RookMoved = false;
        bool isA8RookMoved = false;
        bool isH8RookMoved = false;
        bool isH1RookMoved = false;
        bool isBlackKingMoved = false;
        bool isWhiteKingMoved = false;

        private void Form1_Load(object sender, EventArgs e)
        {

            form = this;
            form.Width = squareEdgePixel*boardHorizontalSquareCount + formLeftCut + formRightCut;
            form.Height = squareEdgePixel*boardVerticalSquareCount + formUpCut + formDownCut;
            board = new Square[boardHorizontalSquareCount, boardVerticalSquareCount];

            Panel topPanel = new Panel();
            topPanel.MouseDown += topBar_MouseDown;
            topPanel.Height = 40;
            topPanel.Width = form.Width;
            topPanel.Anchor = AnchorStyles.Top;
            topPanel.BackColor = Color.FromArgb(30, 30, 30);
            form.Controls.Add(topPanel);

            for (int line = 0; line < board.GetLength(0); line++)
            {
                for (int column = 0; column < board.GetLength(1); column++)
                {
                    var newSquare = new Square();
                    newSquare.BackColor = ((line+column)%2==1) ? squareColorDark : squareColorLight;
                    newSquare.Width = squareEdgePixel;
                    newSquare.Height = squareEdgePixel;
                    newSquare.Left = formLeftCut + column*squareEdgePixel;
                    newSquare.Top = formUpCut + line*squareEdgePixel;
                    newSquare.BackgroundImageLayout = ImageLayout.Stretch;
                    newSquare.lineNumber = line;
                    newSquare.columnNumber = column;
                    newSquare.Piece = Pieces.EMPTY;
                    board[line, column] = newSquare;
                    newSquare.Click += SquareClick;
                    form.Controls.Add(newSquare);
                }
            }

            NewBackground();
            NewBoard();

        }

        Square LastFocusedSquare;
        Square CurrentFocusedSquare;

        private void SquareClick(object sender, EventArgs e)
        {
            NewBackground();
            LastFocusedSquare = CurrentFocusedSquare;
            CurrentFocusedSquare = sender as Square;
            CurrentFocusedSquare.BackColor = focusColor;
            if (LastFocusedSquare != null)
            {
                if (!Move(board, LastFocusedSquare, CurrentFocusedSquare)) HighlightPossibleMoves();
                else
                {
                    // move occured
                    //MessageBox.Show(ifCheck(board, LastFocusedSquare.PieceColor).ToString());
                    if (ifCheck(board, LastFocusedSquare.PieceColor==PieceColors.BLACK?PieceColors.WHITE:PieceColors.BLACK))
                        Check(LastFocusedSquare.PieceColor==PieceColors.BLACK ? PieceColors.WHITE : PieceColors.BLACK); //check occured

                    LastFocusedSquare = null;
                    CurrentFocusedSquare = null;

                }

            }
            else HighlightPossibleMoves();

        }

        private void Check(PieceColors to)
        {
            bool moveExists = false;
            foreach (var piece in board) {
                if (piece.PieceColor != to || piece.Piece == Pieces.EMPTY) continue;
                for (int line = 0; line < board.GetLength(0); line++)
                {
                    for (int column = 0; column < board.GetLength(1); column++)
                    {
                        if (isMoveLegal(board, piece, board[line, column]))
                        {
                            moveExists = true;
                            break;
                        }
                    }
                }
            }

            if (!moveExists) { MessageBox.Show("Mat!"); }
        }

        void NewBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                board[6, i].PlacePiece(Pieces.PAWN, PieceColors.WHITE);
                board[1, i].PlacePiece(Pieces.PAWN, PieceColors.BLACK);
            }

            board[7, 0].PlacePiece(Pieces.ROOK, PieceColors.WHITE);
            board[7, 7].PlacePiece(Pieces.ROOK, PieceColors.WHITE);
            board[0, 7].PlacePiece(Pieces.ROOK, PieceColors.BLACK);
            board[0, 0].PlacePiece(Pieces.ROOK, PieceColors.BLACK);

            board[7, 1].PlacePiece(Pieces.KNIGHT, PieceColors.WHITE);
            board[0, 1].PlacePiece(Pieces.KNIGHT, PieceColors.BLACK);
            board[7, 6].PlacePiece(Pieces.KNIGHT, PieceColors.WHITE);
            board[0, 6].PlacePiece(Pieces.KNIGHT, PieceColors.BLACK);

            board[7, 2].PlacePiece(Pieces.BISHOP, PieceColors.WHITE);
            board[0, 2].PlacePiece(Pieces.BISHOP, PieceColors.BLACK);
            board[7, 5].PlacePiece(Pieces.BISHOP, PieceColors.WHITE);
            board[0, 5].PlacePiece(Pieces.BISHOP, PieceColors.BLACK);

            board[7, 3].PlacePiece(Pieces.QUEEN, PieceColors.WHITE);
            board[0, 3].PlacePiece(Pieces.QUEEN, PieceColors.BLACK);
            board[7, 4].PlacePiece(Pieces.KING, PieceColors.WHITE);
            board[0, 4].PlacePiece(Pieces.KING, PieceColors.BLACK);
        }


    }
}
