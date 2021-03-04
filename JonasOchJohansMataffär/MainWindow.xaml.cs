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
    //Class for handling methods and variables related to the Store
    public class Store
    {
        //Variables
        public Image articleImage;
        public ComboBox articleList;
        public TextBlock titleHeader;
        public Label articleDescription;
        public TextBox storeAmount;
        public Label priceLabel;
        public Button addToCartButton;

        //Methods
    }
    public class Cart
    {

    }
    public class CSVHandler
    {

    }
    public static class CSVutility
    {
        public static void ToCSV(this DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers
            //for (int i = 0; i < dtDataTable.Columns.Count; i++)
            //{
            //    sw.Write(dtDataTable.Columns[i]);
            //    if (i < dtDataTable.Columns.Count - 1)
            //    {
            //        sw.Write(";");
            //    }
            //}
            //sw.Write(sw.NewLine);
            foreach (DataRow row in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(row[i]))
                    {
                        sw.Write(row[i].ToString());
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(";");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
    }

    public class Product
    {
        public string ArticleName { get; set; }
        public decimal ArticlePrice { get; set; }
        public string ArticleDescription { get; set; }
    }

    public partial class MainWindow : Window
    {
        public DataColumn cartAmount;
        public DataColumn isDeleted;
        public TextBox discountCode;
        public List<string[]> file = File.ReadLines(@"Documents\utbud.csv").Select(a => a.Split(';')).ToList();
        public List<Product> products = new List<Product>();
        public DataTable tableForCart;
        public DataGrid gridForCart;
        public Label totalLabel;
        public int totalItems = 0;
        public decimal totalPrice = 0;
        public Dictionary<string, decimal> discountCoupons = new Dictionary<string, decimal>();
        public List<string> usedDiscountCoupons = new List<string>();

        Store STORE = new Store();
        Cart cart = new Cart();

        public MainWindow()
        {
            InitializeComponent();
            Start();
            Closed += MainWindow_Closed;
            if (MessageBox.Show("Would you like to continue on your last cart?", "Cart", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                LoadCart();
            }
            UpdateTotals();
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
            grid.RowDefinitions.Add(new RowDefinition { MaxHeight = 50 });
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
            STORE.articleImage = new Image
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
                Stretch = Stretch.UniformToFill,
                Width = 250,
                Height = 250,
                Source = ReadImage(@"Pictures\Placeholder.jpg")
            };
            store.Children.Add(STORE.articleImage);
            //Grid for both choosing articles and description
            Grid articles = new Grid();
            articles.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Star) });
            articles.RowDefinitions.Add(new RowDefinition { Height = new GridLength(75, GridUnitType.Star) });
            articles.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Star) });
            articles.ColumnDefinitions.Add(new ColumnDefinition());
            store.Children.Add(articles);
            //Combobox to choose article
            STORE.articleList = new ComboBox
            {
                Name = "Articles",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                ItemsSource = products.Select(products => products.ArticleName)
            };
            STORE.articleList.DropDownOpened += ArticleList_DropDownOpened;
            STORE.articleList.SelectionChanged += ArticleList_SelectionChanged;
            articles.Children.Add(STORE.articleList);
            //Header over article list
            STORE.titleHeader = new TextBlock
            {
                Text = "Article",
                IsHitTestVisible = false,
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            articles.Children.Add(STORE.titleHeader);
            //Label to describe the chosen article
            STORE.articleDescription = new Label
            {
                Content = "Description of articles",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            articles.Children.Add(STORE.articleDescription);
            Grid.SetRow(STORE.articleDescription, 1);
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
            STORE.priceLabel = new Label
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "Price:"
            };
            addToCartGrid.Children.Add(STORE.priceLabel);
            Grid.SetColumnSpan(STORE.priceLabel, 4);
            //Amount to add to cart, default is one
            STORE.storeAmount = new TextBox
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Text = "1",
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            STORE.storeAmount.TextChanged += CheckForMinimumAmount;
            STORE.storeAmount.GotFocus += SelectionStartAmount;
            addToCartGrid.Children.Add(STORE.storeAmount);
            STORE.storeAmount.KeyDown += Integers_KeyDown;
            Grid.SetColumn(STORE.storeAmount, 1);
            Grid.SetRow(STORE.storeAmount, 1);
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
            STORE.addToCartButton = new Button
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "Add to cart",
                IsEnabled = false
            };
            STORE.addToCartButton.Click += AddToCartButton_Click;
            addToCartGrid.Children.Add(STORE.addToCartButton);
            Grid.SetColumn(STORE.addToCartButton, 3);
            Grid.SetRow(STORE.addToCartButton, 1);
            #endregion

            // Main cart grid
            #region
            Grid cartGrid = new Grid();
            grid.Children.Add(cartGrid);
            Grid.SetColumn(cartGrid, 1);
            Grid.SetRow(cartGrid, 1);
            cartGrid.Margin = new Thickness(5);
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(90, GridUnitType.Star) });
            cartGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10, GridUnitType.Star) });
            gridForCart = new DataGrid
            {
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                CanUserReorderColumns = false,
                CanUserResizeColumns = false,
                CanUserResizeRows = false,
                CanUserSortColumns = false
            };
            gridForCart.CellEditEnding += GridForCart_CellEditEnding;
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
                ReadOnly = false,
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
            checkOutGrid.RowDefinitions.Add(new RowDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            checkOutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            //labels for total price and total amount
            totalLabel = new Label
            {
                Content = "Totals",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            checkOutGrid.Children.Add(totalLabel);
            Grid.SetColumnSpan(totalLabel, 5);
            //discount label
            Label discountLabel = new Label
            {
                Content = "Coupon:",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            checkOutGrid.Children.Add(discountLabel);
            Grid.SetRow(discountLabel, 1);
            //discount textbox
            discountCode = new TextBox
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            //Testa lambda ternery
            //discountCode.KeyDown += DiscountCode_KeyDown;
            checkOutGrid.Children.Add(discountCode);
            Grid.SetColumn(discountCode, 1);
            Grid.SetRow(discountCode, 1);
            Button addDiscountCode = new Button
            {
                Content = "Enter",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            checkOutGrid.Children.Add(addDiscountCode);
            addDiscountCode.Click += AddDiscountCode;
            Grid.SetColumn(addDiscountCode, 2);
            Grid.SetRow(addDiscountCode, 1);
            // Print receipt and pay for cart
            Button payButton = new Button
            {
                Content = "Pay",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            payButton.Click += PayButton_Click;
            checkOutGrid.Children.Add(payButton);
            Grid.SetColumn(payButton, 3);
            Grid.SetRow(payButton, 1);
            //Clear all
            Button clearAllCart = new Button
            {
                Content = "Delete All",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            clearAllCart.Click += delegate { tableForCart.Rows.Clear(); };
            checkOutGrid.Children.Add(clearAllCart);
            Grid.SetColumn(clearAllCart, 4);
            Grid.SetRow(clearAllCart, 1);
            #endregion
            discountCoupons.Add("code10", 0.1M);
            discountCoupons.Add("code15", 0.15M);
            discountCoupons.Add("code20", 0.2M);
        }

        private void PayButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hejdå");
        }

        private void AddDiscountCode(object sender, RoutedEventArgs e)
        {
            string inputdiscount = discountCode.Text.ToLower();
            if (discountCoupons.ContainsKey(inputdiscount) && !usedDiscountCoupons.Contains(inputdiscount))
            {
                usedDiscountCoupons.Add(inputdiscount);
            }
            else if (discountCoupons.ContainsKey(inputdiscount) && usedDiscountCoupons.Contains(inputdiscount))
            {
                MessageBox.Show("Coupon is already in use", "Coupon", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            discountCode.Text = "";
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            totalItems = 0;
            totalPrice = 0;
            decimal totalDiscount = 0.0M;
            foreach (DataRow row in tableForCart.AsEnumerable())
            {
                totalItems += int.Parse(row[2].ToString());
                totalPrice += decimal.Parse(row[1].ToString());
            }
            foreach (string coupon in usedDiscountCoupons)
            {
                totalDiscount += discountCoupons[coupon];
            }
            totalLabel.Content = $"Total quantity: {totalItems} pcs Total price: {totalPrice:N2}kr\n" +
                                 $"Total price after discount coupons: {totalPrice * (1 - totalDiscount):N2}kr";
        }

        private void LoadCart()
        {
            if (File.Exists(@"C:\Windows\Temp\cart.txt"))
            {
                List<string[]> lines = File.ReadLines(@"c:\Windows\Temp\cart.txt").Select(a => a.Split(';')).ToList();
                foreach (var line in lines)
                {
                    DataRow newRow = tableForCart.NewRow();
                    newRow[0] = line[0];
                    newRow[1] = line[1];
                    newRow[2] = line[2];
                    newRow[3] = false;
                    tableForCart.Rows.Add(newRow);
                }
                MessageBox.Show("Loaded succesfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Couldn't find latest cart", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (tableForCart.Rows.Count > 1)
            {
                tableForCart.ToCSV(@"C:\Windows\Temp\cart.txt");
            }
        }

        private void GridForCart_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if ("Amount" == e.Column.Header.ToString() || "Price" == e.Column.Header.ToString())
            {
                totalItems = 0;
                totalPrice = 0;
                //Kollar så priset matchar produktpriset och om isDeleted är incheckat
                foreach (var row in tableForCart.AsEnumerable())
                {
                    int correctAmount = int.Parse(row[2].ToString());
                    if (int.TryParse(((TextBox)e.EditingElement).Text.ToString(), out int newAmount) && "Amount" == e.Column.Header.ToString() && tableForCart.Rows.IndexOf(row) == gridForCart.SelectedIndex)
                    {
                        correctAmount = newAmount;
                    }
                    var productNames = products.Select(products => products.ArticleName).ToList();
                    int indexOfProduct = productNames.IndexOf(row[0].ToString());
                    row[2] = correctAmount;
                    row[1] = correctAmount * products[indexOfProduct].ArticlePrice;
                }
            }
            else if (e.Column.Header.ToString() == "Delete")
            {
                var checkBox = (CheckBox)e.EditingElement;
                //If deleted is check, remove row
                if (e.Column.Header.ToString() == "Delete" && (bool)checkBox.IsChecked)
                {
                    tableForCart.Rows.RemoveAt(gridForCart.SelectedIndex);
                }
            }
            UpdateTotals();
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < int.Parse(STORE.storeAmount.Text); i++)
            {
                bool exists = tableForCart.AsEnumerable().Any(row => row.Field<string>("Article Name") == products[STORE.articleList.SelectedIndex].ArticleName);
                if (!exists)
                {
                    DataRow newRow = tableForCart.NewRow();
                    newRow[0] = products[STORE.articleList.SelectedIndex].ArticleName;
                    newRow[1] = products[STORE.articleList.SelectedIndex].ArticlePrice;
                    newRow[2] = 1;
                    newRow[3] = false;
                    tableForCart.Rows.Add(newRow);
                }
                else
                {
                    //Söker och tar fram raden som matchar artikelnamnet, använder first eftersom vi utgår från att det enbart finns en av de namnet och vi vill enbart ha en rad att arbeta med.
                    DataRow result = tableForCart.Select().Where(row => row.Field<string>("Article Name") == products[STORE.articleList.SelectedIndex].ArticleName).First();
                    int newAmount = int.Parse(result[2].ToString()) + 1;
                    result[2] = newAmount;
                    result[1] = newAmount * products[STORE.articleList.SelectedIndex].ArticlePrice;
                }
            }
            UpdateTotals();
            STORE.storeAmount.Text = "1";
        }

        private void ArticleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            STORE.articleImage.Source = ReadImage(Path.Combine(@"Pictures\", STORE.articleList.SelectedItem.ToString() + ".jpg"));
            STORE.priceLabel.Content = "Price: " + products[STORE.articleList.SelectedIndex].ArticlePrice + " SEK";
            STORE.articleDescription.Content = "";
            STORE.addToCartButton.IsEnabled = true;
        }

        private void ArticleList_DropDownOpened(object sender, EventArgs e)
        {
            STORE.titleHeader.Text = "";
        }

        //event handlers
        #region

        private void SelectionStartAmount(object sender, RoutedEventArgs e)
        {
            STORE.storeAmount.SelectionStart = STORE.storeAmount.Text.Length;
            STORE.storeAmount.SelectionLength = 0;
        }

        private void CheckForMinimumAmount(object sender, TextChangedEventArgs e)
        {
            int.TryParse(STORE.storeAmount.Text, out int currentAmount);
            if (STORE.storeAmount.Text.Length > 0 && currentAmount < 1)
            {
                STORE.storeAmount.Text = "1";
            }
        }

        private void IncreaseAmount_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(STORE.storeAmount.Text, out int currentAmount);
            currentAmount++;
            STORE.storeAmount.Text = currentAmount.ToString();
        }

        private void DecreaseAmount_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(STORE.storeAmount.Text, out int currentAmount);
            if (currentAmount < 1)
            {
                STORE.storeAmount.Text = "1";
            }
            else
            {
                currentAmount--;
                STORE.storeAmount.Text = currentAmount.ToString();
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