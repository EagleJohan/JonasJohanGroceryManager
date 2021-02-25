using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JonasOchJohansMataffär
{
    public class Product
    {
        public string ArticleName { get; set; }
        public decimal ArticlePrice { get; set; }
        public string ArticleDescription { get; set; }
    }
    public partial class MainWindow : Window
    {
        public Image articleImage;
        public ComboBox articleList;
        public TextBlock titleHeader;
        public Label articleDescription;
        public TextBox storeAmount;
        public DataColumn cartAmount;
        public DataColumn isDeleted;
        public TextBox discountCode;
        public Label priceLabel;
        public Button addToCartButton;
        public List<string[]> file = File.ReadLines(@"Documents\utbud.csv").Select(a => a.Split(';')).ToList();
        public List<Product> products = new List<Product>();
        public List<Product> cart = new List<Product>();
        public DataTable tableForCart;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            //Read Cart and business offerings
            foreach (var line in file)
            {
                Product product = new Product
                {
                    ArticleName = line[1],
                    ArticlePrice = decimal.Parse(line[2])
                };
                products.Add(product);
            }
            // Window options
            Title = "Generic Store AB";
            SizeToContent = SizeToContent.Height;
            Width = 1000;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            Grid grid = new Grid
            {
                Margin = new Thickness(5)
            };
            root.Content = grid;
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            //Titles for store and cart
            Label storeTitle = new Label
            {
                Content = "Affär",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 20,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            grid.Children.Add(storeTitle);

            Label cartTitle = new Label
            {
                Content = "Kundvagn",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 20,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            grid.Children.Add(cartTitle);
            Grid.SetColumn(cartTitle, 1);
            // Store grid
            #region
            WrapPanel store = new WrapPanel
            {
                Orientation = Orientation.Horizontal
            };
            grid.Children.Add(store);
            store.Margin = new Thickness(5);
            store.Children.Add(CreateImage("bild"));
            Grid.SetRow(store, 1);

            //A panel for article images
            articleImage = new Image
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
                Stretch = Stretch.UniformToFill,
                Width = 250,
                Height = 250,
                Source = ReadImage(@"Pictures\Placeholder.jpg")
            };
            store.Children.Add(articleImage);
            //Grid for both choosing articles and description
            Grid articles = new Grid();
            articles.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Star) });
            articles.RowDefinitions.Add(new RowDefinition { Height = new GridLength(75, GridUnitType.Star) });
            articles.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Star) });
            articles.ColumnDefinitions.Add(new ColumnDefinition());
            store.Children.Add(articles);
            //Combobox to choose article
            articleList = new ComboBox
            {
                Name = "Articles",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                ItemsSource = products.Select(products => products.ArticleName)
            };
            articleList.DropDownOpened += ArticleList_DropDownOpened;
            articleList.SelectionChanged += ArticleList_SelectionChanged;
            articles.Children.Add(articleList);
            //Header over article list
            titleHeader = new TextBlock
            {
                Text = "Articles",
                IsHitTestVisible = false,
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            articles.Children.Add(titleHeader);
            //Label to describe the chosen article
            articleDescription = new Label
            {
                Content = "Description of articles",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            articles.Children.Add(articleDescription);
            Grid.SetRow(articleDescription, 1);
            //Grid for adding articles to cart
            Grid addToCartGrid = new Grid();
            articles.Children.Add(addToCartGrid);
            Grid.SetRow(addToCartGrid, 2);
            addToCartGrid.RowDefinitions.Add(new RowDefinition { });
            addToCartGrid.RowDefinitions.Add(new RowDefinition { });
            addToCartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) });
            addToCartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60, GridUnitType.Star) });
            addToCartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) });
            addToCartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });
            //Price of current article
            priceLabel = new Label
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "Price:"
            };
            addToCartGrid.Children.Add(priceLabel);
            Grid.SetColumnSpan(priceLabel, 4);
            //Amount to add to cart, default is one
            storeAmount = new TextBox
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Text = "1",
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            storeAmount.TextChanged += CheckForMinimumAmount;
            storeAmount.GotFocus += SelectionStartAmount;
            addToCartGrid.Children.Add(storeAmount);
            storeAmount.KeyDown += Integers_KeyDown;
            Grid.SetColumn(storeAmount, 1);
            Grid.SetRow(storeAmount, 1);
            //Button to decrease amount
            Button decreaseAmount = new Button
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "-"
            };
            decreaseAmount.Click += DecreaseAmount_Click;
            addToCartGrid.Children.Add(decreaseAmount);
            Grid.SetRow(decreaseAmount, 1);
            //button to increase amount
            Button increaseAmount = new Button
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "+"
            };
            increaseAmount.Click += IncreaseAmount_Click;
            addToCartGrid.Children.Add(increaseAmount);
            Grid.SetColumn(increaseAmount, 2);
            Grid.SetRow(increaseAmount, 1);
            //button to add to cart
            addToCartButton = new Button
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "Add to cart",
                IsEnabled = false
            };
            addToCartButton.Click += AddToCartButton_Click;
            addToCartGrid.Children.Add(addToCartButton);
            Grid.SetColumn(addToCartButton, 3);
            Grid.SetRow(addToCartButton, 1);
            #endregion

            // Main cart grid
            #region
            Grid cartGrid = new Grid();
            grid.Children.Add(cartGrid);
            Grid.SetColumn(cartGrid, 1);
            Grid.SetRow(cartGrid, 1);
            cartGrid.Margin = new Thickness(5);
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(90, GridUnitType.Star)});
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10, GridUnitType.Star) });

            // Cart with price and description
            DataGrid gridForCart = new DataGrid();
            cartGrid.Children.Add(gridForCart);
            //Datatable for handling articles in customers cart
            tableForCart = new DataTable();
            //Fixed values has readonly set to true
            tableForCart.Columns.Add(new DataColumn
            {
                ReadOnly = true,
                ColumnName = "Article Name",
                DataType = typeof(string)
            });
            tableForCart.Columns.Add(new DataColumn
            {
                ReadOnly = true,
                ColumnName = "Price",
                DataType = typeof(decimal)
            });
            // amount and delete is dynamic
            cartAmount = new DataColumn
            {
                ColumnName = "Amount",
                DataType = typeof(int)
            };
            tableForCart.Columns.Add(cartAmount);
            isDeleted = new DataColumn
            {
                ColumnName = "Delete",
                DataType = typeof(bool)
            };
            tableForCart.Columns.Add(isDeleted);
            gridForCart.ItemsSource = tableForCart.DefaultView;
            //Grid for discount codes, clear shopping cart and print receipt
            Grid checkOutGrid = new Grid();
            cartGrid.Children.Add(checkOutGrid);
            Grid.SetRow(checkOutGrid, 1);
            checkOutGrid.RowDefinitions.Add(new RowDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            //discount label
            Label discountLabel = new Label
            {
                Content = "Coupon:",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            checkOutGrid.Children.Add(discountLabel);
            //discount textbox
            discountCode = new TextBox
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            checkOutGrid.Children.Add(discountCode);
            Grid.SetColumn(discountCode, 1);
            // Print receipt and pay for cart
            Button printReceipt = new Button
            {
                Content = "Pay",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            checkOutGrid.Children.Add(printReceipt);
            Grid.SetColumn(printReceipt, 2);
            //Clear all
            Button clearAllCart = new Button
            {
                Content = "Delete All",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            checkOutGrid.Children.Add(clearAllCart);
            Grid.SetColumn(clearAllCart, 3);
            #endregion
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < int.Parse(storeAmount.Text); i++)
            {
                bool exists = tableForCart.AsEnumerable().Any(row => row.Field<string>("Article Name") == products[articleList.SelectedIndex].ArticleName);
                if (!exists)
                {
                    DataRow newRow = tableForCart.NewRow();
                    newRow[0] = products[articleList.SelectedIndex].ArticleName;
                    newRow[1] = products[articleList.SelectedIndex].ArticlePrice;
                    newRow[2] = 1;
                    newRow[3] = false;
                    tableForCart.Rows.Add(newRow);

                }
                else
                {
                    //Söker och tar fram raden som matchar artikelnamnet, använder first eftersom vi utgår från att det enbart finns en av de namnet och vi vill enbart ha en rad att arbeta med.
                    DataRow result = tableForCart.Select().Where(row => row.Field<string>("Article Name") == products[articleList.SelectedIndex].ArticleName).First();
                    int newAmount = int.Parse(result[2].ToString()) + 1;
                    result[2] = newAmount;
                }
            }
            Button plus = new Button { Content = "+", Margin = new Thickness(5) };

            Button minus = new Button { Content = "-", Margin = new Thickness(5) };

            ImageSource source = new BitmapImage(new Uri(@"Pictures\Trashcan.png", UriKind.Relative));
            Image trashCan = new Image
            {
                Source = source,
                Width = 25,
                Height = 25,
                Stretch = Stretch.UniformToFill,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };

            Button delete = new Button { Content = trashCan, Margin = new Thickness(5) };
        }

        private void ArticleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            articleImage.Source = ReadImage(Path.Combine(@"Pictures\", articleList.SelectedItem.ToString() + ".jpg"));
            priceLabel.Content = "Price: " + products[articleList.SelectedIndex].ArticlePrice + " SEK";
            articleDescription.Content = "";
            addToCartButton.IsEnabled = true;
        }

        private void ArticleList_DropDownOpened(object sender, EventArgs e)
        {
            titleHeader.Text = "";
        }

        //event handlers
        #region

        private void SelectionStartAmount(object sender, RoutedEventArgs e)
        {
            storeAmount.SelectionStart = storeAmount.Text.Length;
            storeAmount.SelectionLength = 0;
        }

        private void CheckForMinimumAmount(object sender, TextChangedEventArgs e)
        {
            int.TryParse(storeAmount.Text, out int currentAmount);
            if (storeAmount.Text.Length > 0 && currentAmount < 1)
            {
                storeAmount.Text = "1";
            }
        }

        private void IncreaseAmount_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(storeAmount.Text, out int currentAmount);
            currentAmount++;
            storeAmount.Text = currentAmount.ToString();
        }

        private void DecreaseAmount_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(storeAmount.Text, out int currentAmount);
            if (currentAmount < 1)
            {
                storeAmount.Text = "1";
            }
            else
            {
                currentAmount--;
                storeAmount.Text = currentAmount.ToString();
            }
        }

        /// <summary>
        /// Check if key pressed is a digit from numpad or digitkeys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Integers_KeyDown(object sender, KeyEventArgs e)
        {
            var digitkeys = e.Key >= Key.D0 && e.Key <= Key.D9;
            var numbpadKeys = e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9;
            var modifiedKey = e.KeyboardDevice.Modifiers == ModifierKeys.None;
            if (modifiedKey && (digitkeys || numbpadKeys))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Creates image from source in project filepath/filename
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private Image CreateImage(string filePath)
        {
            ImageSource source = new BitmapImage(new Uri(filePath, UriKind.Relative));
            Image image = new Image
            {
                Source = source,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
                Stretch = Stretch.None,
            };
            // A small rendering tweak to ensure maximum visual appeal.
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            return image;
        }

        private ImageSource ReadImage(string fileName)
        {
            ImageSource source = new BitmapImage(new Uri(fileName, UriKind.Relative));
            return source;
        }

        #endregion
    }
}