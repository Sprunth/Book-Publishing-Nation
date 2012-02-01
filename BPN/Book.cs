using System;
using System.Collections.Generic;
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
        private static List<Book> archivedBooks = new List<Book>();
        public static List<Book> ArchivedBooks { get { return archivedBooks; } }
        public enum BookStatus { Review, Approved, Selling, Retired, Denied };

        private static int idCounter = 1;
        public static int IdCounter { get { return idCounter; } }

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
            menu.SetSize(160, 2*P.ScreenSize.Y/3 - 36);
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
            publishingProgress.SetBounds(300, P.ScreenSize.Y/3-16, 200, 16);
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
                Debug.WriteLine("Selected: " + menu.SelectedRow.Text);

                menu.SelectedRow.SetTextColor(System.Drawing.Color.White);

                //Assigns the active book to the selected menu item
                foreach (Book b in activeBooks)
                {
                    if (b.title.Equals(menu.SelectedRow.Text))
                    { 
                        activeBook = b; 
                        Debug.WriteLine("Activebook: " + activeBook.title);
                    }
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
            if (menu.SelectedRows.Count == 0)
            {
                publishingProgress.Hide();
            }

            #region
            List<Book> toArchive = new List<Book>();
            foreach (Book b in activeBooks)
            {
                if ((GameManager.FrameCounter % 30) == 0)
                {
                    //Debug.WriteLine(b.title + " framesPast " + b.framesPast + " framesMax " + b.framesTillStatusChange);
                }

                b.framesPast++;

                if (b.status != BookStatus.Retired)
                {
                    b.framesPast++;
                    b.progress = (b.framesPast * 1.0F) / b.framesTillStatusChange;

                    //Assign progress bar value
                    if (menu.SelectedRows.Count != 0)
                    { publishingProgress.Value = activeBook.progress; }
                }

                #region chaning Book states
                if (b.framesPast >= b.framesTillStatusChange)
                {
                    switch (b.status)
                    {
                        case BookStatus.Review:
                            {
                                b.status = BookStatus.Denied;
                                toArchive.Add(b);

                                DeleteBookFromMenu(b);

                                Notifications.AddNotification(b.title + " has exceeded its waiting period and has been automatically denied.");
                                break;
                            }
                        case BookStatus.Approved:
                            {
                                b.status = BookStatus.Selling;
                                b.framesTillStatusChange = b.sellingDifficulty * difficultyIndex;
                                b.framesPast = 0;
                                Notifications.AddNotification(b.title + " has been published! It is now on the market!");
                                break;
                            }
                        case BookStatus.Selling:
                            {
                                b.status = BookStatus.Retired;
                                b.framesPast = 0;
                                Notifications.AddNotification(b.title + " has finished selling. It is now off the market.");
                                break;
                            }
                    }
                }
                #endregion
            }
            #endregion
            
            #region Handle Book Archiving
            /*
                List<int> indexesToDelete = new List<int>();

                //archieves denied and retired books
                foreach (Book b in toArchive)
                {
                    activeBooks.Remove(b);
                    archivedBooks.Add(b);

                    for (int i = 0; i < activeBookItems.Count; i++)
                    {
                        if (activeBookItems[i].Text == b.title)
                        {
                            indexesToDelete.Add(i);
                        }
                    }
                }
                int removedCount = 0;
                foreach (int g in indexesToDelete)
                {
                    menu.RemoveRow(g);
                    removedCount++;
                }

                toArchive.Clear();
            */
            #endregion

            //need to remove from ActiveBooks
            foreach (Book d in toArchive)
            { activeBooks.Remove(d); }

        }
        #endregion

        private static void DeleteBookFromMenu(Book b)
        {
            int indexToDelete = -1;

            //archieves denied and retired books
            archivedBooks.Add(b);

            for (int i = 0; i < activeBookItems.Count; i++)
            {
                if (activeBookItems[i].Text == b.title)
                {
                    indexToDelete = i;
                }
            }

            try
            {
                menu.RemoveRow(indexToDelete);
            }
            catch (Exception e)
            { Debug.WriteLine(":O " + e); }

        }

        private BookStatus status;
        public BookStatus Status { get { return status; } }
        private string title;
        public string Title { get { return title; } }
        private string[] author = new string[2];
        public string Author { get { return author[0] + " " + author[1]; } }
        private float progress;
        public float Progress { get { return progress; } }
        private int reviewDifficulty;
        public int ReviewDifficulty { get { return reviewDifficulty; } }
        private int publishingDifficulty;
        public int PublishingDifficulty { get { return publishingDifficulty; } }
        private int sellingDifficulty;
        public int SellingDifficulty { get { return sellingDifficulty; } }
        private int framesTillStatusChange, framesPast = 0;
        private const int difficultyIndex = 60;
        private int _ID;
        public int ID { get { return _ID; } }

        public Book()
        {
            Debug.WriteLine("Generating new Book");
            Thread.Sleep(50);
            title = BookNameGenerator.NextBookTitle();
            status = BookStatus.Review;
            progress = 0.0F;
            author = PersonNameGenerator.NextPersonName();
            reviewDifficulty = P.Randomizer.Next(15, 45);
            Debug.WriteLine("review difficulty for " + title + ": " + reviewDifficulty);
            publishingDifficulty = P.Randomizer.Next(45, 120) + reviewDifficulty;
            sellingDifficulty = P.Randomizer.Next(120, 300) + publishingDifficulty;
            framesTillStatusChange = reviewDifficulty*difficultyIndex;
            _ID = idCounter;
            idCounter++;

            activeBooks.Add(this);
        }

        [Obsolete]
        public void UpdateBook()
        {
            if (status != BookStatus.Retired)
            {
                framesPast++;
                progress = (framesPast*1.0F) / framesTillStatusChange;
                publishingProgress.Value = progress;
            }

            if (framesPast >= framesTillStatusChange)
            {
                switch (this.status)
                {
                    case BookStatus.Review: 
                        {
                            this.status = BookStatus.Approved;
                            framesTillStatusChange = publishingDifficulty*difficultyIndex;
                            framesPast = 0;
                            Notifications.AddNotification(this.title + " has been approved and the publishing segment has started.");
                            break; 
                        }
                    case BookStatus.Approved:
                        {
                            this.status = BookStatus.Selling;
                            framesTillStatusChange = sellingDifficulty * difficultyIndex;
                            framesPast = 0;
                            Notifications.AddNotification(this.title + " has been published! It is now on the market!");
                            break;
                        }
                    case BookStatus.Selling:
                        {
                            this.status = BookStatus.Retired;
                            framesPast = 0;
                            Notifications.AddNotification(this.title + " has finished selling. It is now off the market.");
                            break;
                        }
                }
            }
        }

    }
}
