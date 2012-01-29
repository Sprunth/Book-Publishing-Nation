using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Gwen.Controls;
using Gwen.ControlsInternal;
using Gwen.Controls.Layout;
using SFML.Window;
using System.Diagnostics;

namespace BPN
{
    using P = Program;
    
    class Book
    {
        #region static
        private static List<Book> activeBooks = new List<Book>();
        public static List<Book> ActiveBooks { get { return activeBooks; } }
        public enum BookStatus { Review, Approved, Selling, Retired };

        /// <summary>
        /// A list of all the active books for the GUI. (It is the row item)
        /// </summary>
        private static List<TableRow> activeBookItems = new List<TableRow>();

        private static ListBox menu;
        public static ListBox Menu { get { return menu; } }
        private static Text menuLabel;
        public static Text MenuLabel { get { return menuLabel; } }

        /// <summary>
        /// The selected book. This is used for the gui items on the right displaying the info.
        /// </summary>
        private static Book activeBook;
        public static Book ActiveBook { get { return activeBook; } }

        private static Text bookTitle;
        public static Text BookTitle { get { return bookTitle; } }
        private static Text statusText;
        public static Text StatusText { get { return statusText; } }
        private static ProgressBar publishingProgress;
        public static ProgressBar PublishingProgress { get { return publishingProgress; } }
        private static Text authorText;
        public static Text AuthorText { get { return authorText; } }

        public static void Initialize()
        {
            #region menu setup
            menuLabel = new Text(P._Canvas);
            menuLabel.SetPos(128, 28);
            menuLabel.Font = P.GuiFontLarge;
            menuLabel.String = "Books";

            menu = new ListBox(P._Canvas);
            foreach (Book b in activeBooks)
            {
                activeBookItems.Add(menu.AddItem(b.title));
            }
            menu.EnableScroll(true, true);
            menu.SetSize(160, P.ScreenSize.Y-24*3);
            menu.SetPos(128,48);
            
            menu.MouseInputEnabled = true;
            for (int i = 0; i < activeBookItems.Count; i++)
            {
                activeBookItems[i].OnRowSelected += new Base.ControlCallback(Book_OnRowSelected);
                activeBookItems[i].SetTextColor(System.Drawing.Color.Gray);
            }
            #endregion

            bookTitle = new Text(P._Canvas);
            bookTitle.String = "";
            bookTitle.SetPos(300, 40);
            bookTitle.Font = P.GuiFontLarge;

            authorText = new Text(P._Canvas);
            authorText.String = "";
            authorText.SetPos(300, 64);

            statusText = new Text(P._Canvas);
            statusText.SetPos(300, 80);
            statusText.String = "";

            publishingProgress = new ProgressBar(P._Canvas);
            publishingProgress.Value = 0;
            publishingProgress.SetBounds(300, 96, 100, 16);
            publishingProgress.Hide();

            GameManager.BookPageItems.Add(menuLabel);
            GameManager.BookPageItems.Add(menu);
            GameManager.BookPageItems.Add(bookTitle);
            GameManager.BookPageItems.Add(authorText);
            GameManager.BookPageItems.Add(publishingProgress);
            GameManager.BookPageItems.Add(statusText);
            
        }

        static void Book_OnRowSelected(Base control)
        {
            foreach (TableRow b in activeBookItems)
            {
                b.SetTextColor(System.Drawing.Color.Gray);
                bookTitle.String = "";
                authorText.String = "";
                statusText.String = "";
                publishingProgress.Hide();
            }
            if (menu.SelectedRows.Count != 0)
            {
                Debug.WriteLine(menu.SelectedRow.Text);
                foreach (Book b in activeBooks)
                {
                    if (b.title.Equals(menu.SelectedRow.Text))
                    { activeBook = b; }
                }

                bookTitle.String = activeBook.title;
                authorText.String = "Author: " + activeBook.Author;
                publishingProgress.Show();
                publishingProgress.Value = activeBook.progress;
                
                if (activeBook.status == BookStatus.Review)
                {
                    statusText.String = "Status: In " + activeBook.status;
                }
                else
                {
                    statusText.String = "Status: " + activeBook.status;
                }
            }
        }

        public static void Update()
        {
            
        }
        #endregion

        private BookStatus status;
        public BookStatus Status { get { return status; } }
        private string title;
        public string Title { get { return title; } }
        private string[] author = new string[2];
        public string Author { get { return author[0] + " " + author[1]; } }
        private float progress;
        public float Progress { get { return progress; } }
        public Book()
        {
            Debug.WriteLine("Generating new Book");
            Thread.Sleep(50);
            title = BookNameGenerator.NextBookTitle();
            status = BookStatus.Review;
            progress = 0.0F;
            author = PersonNameGenerator.NextPersonName();

            activeBooks.Add(this);
        }

    }
}
