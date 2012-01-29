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

        public static void Initialize()
        {
            menuLabel = new Text(P.BookPage);
            menuLabel.SetPos(4, 4);
            menuLabel.Font = P.GuiFontLarge;
            menuLabel.String = "Books";

            menu = new ListBox(P.BookPage);
            foreach (Book b in activeBooks)
            {
                activeBookItems.Add(menu.AddItem(b.title));
            }
            menu.EnableScroll(false, true);
            menu.SetSize(150, 100);
            menu.SetPos(4,22);
            menu.MouseInputEnabled = true;
            for (int i = 0; i < activeBookItems.Count; i++)
            {
                activeBookItems[i].OnRowSelected += new Base.ControlCallback(Book_OnRowSelected);
            }
            
        }

        static void Book_OnRowSelected(Base control)
        {
            if (menu.SelectedRows.Count != 0)
            {
                Debug.WriteLine(menu.SelectedRow.Text);
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
        private int progress;
        public int Progress { get { return progress; } }
        public Book()
        {
            Debug.WriteLine("Generating new Book");
            Thread.Sleep(50);
            title = BookNameGenerator.NextBookTitle();
            status = BookStatus.Review;
            progress = 0;
            author = PersonNameGenerator.NextPersonName();

            activeBooks.Add(this);
        }

    }
}
