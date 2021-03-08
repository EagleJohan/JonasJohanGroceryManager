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
        public TextBlock articleDescription;
        public TextBox storeAmount;
        public Label priceLabel;
        public Button addToCartButton;

        public List<Product> products = new List<Product>();

        //Methods
        public Grid CreateGrid()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { });
            grid.RowDefinitions.Add(new RowDefinition { MaxHeight = 50 });
            grid.RowDefinitions.Add(new RowDefinition { });

            Label title = new Label
            {
                Content = "Store",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 20,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            grid.Children.Add(title);

            WrapPanel wrapPanel = new WrapPanel
            {
                Orientation = Orientation.Horizontal
            };
            grid.Children.Add(wrapPanel);
            Grid.SetRow(wrapPanel, 1);
            articleImage = new Image
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
                Stretch = Stretch.UniformToFill,
                Width = 250,
                Height = 250,
                Source = Utility.ReadImage(@"Pictures\Placeholder.jpg")
            };
            wrapPanel.Children.Add(articleImage);

            //Grid for both choosing articles and description
            Grid showArticleGrid = new Grid();
            showArticleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Star) });
            showArticleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Star) });
            showArticleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(75, GridUnitType.Star) });
            showArticleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Star) });
            showArticleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Star) });
            showArticleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            wrapPanel.Children.Add(showArticleGrid);

            //Combobox to choose article
            articleList = new ComboBox
            {
                Name = "Articles",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                ItemsSource = products.Select(products => products.ArticleName),
                MaxWidth = 200
            };
            articleList.DropDownOpened += ArticleList_DropDownOpened;
            articleList.SelectionChanged += ArticleList_SelectionChanged;
            showArticleGrid.Children.Add(articleList);

            //Header over article list
            titleHeader = new TextBlock
            {
                Text = "Article",
                IsHitTestVisible = false,
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            showArticleGrid.Children.Add(titleHeader);

            //Label to describe the chosen article
            articleDescription = new TextBlock
            {
                Text = "Description of articles",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 200
            };
            showArticleGrid.Children.Add(articleDescription);
            Grid.SetRow(articleDescription, 1);

            priceLabel = new Label
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "Price:"
            };
            showArticleGrid.Children.Add(priceLabel);
            Grid.SetRow(priceLabel, 2);

            //Grid for adding articles to cart
            Grid addProductGrid = new Grid();
            showArticleGrid.Children.Add(addProductGrid);
            Grid.SetRow(addProductGrid, 4);
            addProductGrid.RowDefinitions.Add(new RowDefinition { });
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) });
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60, GridUnitType.Star) });
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) });
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });

            //Amount to add to cart, default is one
            storeAmount = new TextBox
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Text = "1",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            storeAmount.TextChanged += CheckForMinimumAmount;
            storeAmount.GotFocus += SelectionStartAmount;
            addProductGrid.Children.Add(storeAmount);
            storeAmount.KeyDown += Integers_KeyDown;
            Grid.SetColumn(storeAmount, 1);
            //Button to decrease amount
            Button decreaseAmount = new Button
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "-"
            };
            decreaseAmount.Click += DecreaseAmount_Click;
            addProductGrid.Children.Add(decreaseAmount);
            //button to increase amount
            Button increaseAmount = new Button
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "+"
            };
            increaseAmount.Click += IncreaseAmount_Click;
            addProductGrid.Children.Add(increaseAmount);
            Grid.SetColumn(increaseAmount, 2);
            //button to add to cart
            addToCartButton = new Button
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "Add to cart",
                IsEnabled = false
            };
            addProductGrid.Children.Add(addToCartButton);
            Grid.SetColumn(addToCartButton, 3);
            return grid;
        }

        //Event handler
        private void ArticleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            articleImage.Source = Utility.ReadImage(Path.Combine(@"Pictures\", articleList.SelectedItem.ToString() + ".jpg"));
            priceLabel.Content = "Price: " + products[articleList.SelectedIndex].ArticlePrice + " SEK";
            articleDescription.Text = products[articleList.SelectedIndex].ArticleDescription;
            addToCartButton.IsEnabled = true;
        }

        private void ArticleList_DropDownOpened(object sender, EventArgs e)
        {
            titleHeader.Text = "";
        }

        //event handlers

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
    }

    public class Cart
    {
        //Varibles
        public DataColumn Quantity;

        public DataColumn Deleted;
        public TextBox discountCode;
        public DataTable table;
        public DataGrid dataGrid;
        public Grid grid;
        public Label totalLabel;
        public int totalItems = 0;
        public decimal totalPrice = 0;
        public Dictionary<string, decimal> discountCoupons = new Dictionary<string, decimal>();
        public List<string> usedDiscount = new List<string>();

        public List<Product> products = new List<Product>();
        public Button payButton;

        //Methods
        public Dictionary<string, decimal> ReadDiscountCodes(Dictionary<string, decimal> couponDictionary, string filePath)
        {
            var lines = File.ReadLines(filePath).Select(a => a.Split(';')).ToList();
            foreach (var line in lines)
            {
                couponDictionary.Add(line[0], decimal.Parse(line[1]));
            }
            return couponDictionary;
        }
        public Grid CreateGrid()
        {
            //Creates main grid
            grid = new Grid();
            grid.Margin = new Thickness(5);
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition { MaxHeight = 50 });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(90, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10, GridUnitType.Star) });

            Label title = new Label
            {
                Content = "Cart",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 20,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            grid.Children.Add(title);

            dataGrid = CreateDataGrid();
            grid.Children.Add(dataGrid);
            Grid.SetRow(dataGrid, 1);

            Grid checkOut = CreateCheckOut();
            grid.Children.Add(checkOut);
            Grid.SetRow(checkOut, 2);

            //Return a completed grid
            return grid;
        }

        public DataGrid CreateDataGrid()
        {
            //Creates DataGrid to display cart
            DataGrid grid = new DataGrid
            {
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                CanUserReorderColumns = false,
                CanUserResizeColumns = false,
                CanUserResizeRows = false,
                CanUserSortColumns = false
            };
            //Create datatable to store information to display on datagrid
            table = new DataTable();
            table.Columns.Add(new DataColumn
            {
                ReadOnly = true,
                ColumnName = "Article Name",
                DataType = typeof(string)
            });
            table.Columns.Add(new DataColumn
            {
                ColumnName = "Price",
                DataType = typeof(decimal)
            });
            // amount and delete is dynamic
            Quantity = new DataColumn
            {
                ColumnName = "Amount",
                DataType = typeof(int)
            };
            table.Columns.Add(Quantity);
            Deleted = new DataColumn
            {
                ColumnName = "Delete",
                DataType = typeof(bool)
            };
            table.Columns.Add(Deleted);
            grid.ItemsSource = table.DefaultView;
            return grid;
        }

        public Grid CreateCheckOut()
        {
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            //Shows all totals in a label
            totalLabel = new Label
            {
                Content = "Totals",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            grid.Children.Add(totalLabel);
            Grid.SetColumnSpan(totalLabel, 5);

            //
            //discount label
            Label discountLabel = new Label
            {
                Content = "Coupon:",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            grid.Children.Add(discountLabel);
            Grid.SetRow(discountLabel, 1);
            //discount textbox
            discountCode = new TextBox
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            grid.Children.Add(discountCode);
            Grid.SetColumn(discountCode, 1);
            Grid.SetRow(discountCode, 1);
            Button addDiscountCode = new Button
            {
                Content = "Enter",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            grid.Children.Add(addDiscountCode);
            addDiscountCode.Click += AddDiscountCode;
            Grid.SetColumn(addDiscountCode, 2);
            Grid.SetRow(addDiscountCode, 1);
            // Print receipt and pay for cart
            payButton = new Button
            {
                Content = "Pay",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            grid.Children.Add(payButton);
            Grid.SetColumn(payButton, 3);
            Grid.SetRow(payButton, 1);
            //Clear all
            Button clearAllCart = new Button
            {
                Content = "Delete All",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            clearAllCart.Click += delegate { table.Rows.Clear(); };
            grid.Children.Add(clearAllCart);
            Grid.SetColumn(clearAllCart, 4);
            Grid.SetRow(clearAllCart, 1);
            return grid;
        }

        public void UpdateTotals()
        {
            totalItems = 0;
            totalPrice = 0;
            decimal totalDiscount = 0.0M;
            foreach (DataRow row in table.AsEnumerable())
            {
                totalItems += int.Parse(row[2].ToString());
                totalPrice += decimal.Parse(row[1].ToString());
            }
            foreach (string coupon in usedDiscount)
            {
                totalDiscount += discountCoupons[coupon];
            }
            totalLabel.Content = $"Total quantity: {totalItems} pcs Total price: {totalPrice:N2}kr\n" +
                                 $"Total price after discount coupons: {totalPrice * (1 - totalDiscount):N2}kr";
        }

        public void Load()
        {
            if (File.Exists(@"C:\Windows\Temp\cart.txt"))
            {
                List<string[]> lines = File.ReadLines(@"c:\Windows\Temp\cart.txt").Select(a => a.Split(';')).ToList();
                foreach (var line in lines)
                {
                    DataRow newRow = table.NewRow();
                    newRow[0] = line[0];
                    newRow[1] = line[1];
                    newRow[2] = line[2];
                    newRow[3] = false;
                    table.Rows.Add(newRow);
                }
                MessageBox.Show("Loaded succesfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Couldn't find latest cart", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Event handlers
        public void AddDiscountCode(object sender, RoutedEventArgs e)
        {
            string inputdiscount = discountCode.Text.ToLower();
            if (discountCoupons.ContainsKey(inputdiscount) && !usedDiscount.Contains(inputdiscount))
            {
                usedDiscount.Add(inputdiscount);
            }
            else if (discountCoupons.ContainsKey(inputdiscount) && usedDiscount.Contains(inputdiscount))
            {
                MessageBox.Show("Coupon is already in use", "Coupon", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            discountCode.Text = "";
            UpdateTotals();
        }

        private void GridForCart_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if ("Amount" == e.Column.Header.ToString() || "Price" == e.Column.Header.ToString())
            {
                totalItems = 0;
                totalPrice = 0;
                //Kollar så priset matchar produktpriset och om isDeleted är incheckat
                foreach (var row in table.AsEnumerable())
                {
                    int correctAmount = int.Parse(row[2].ToString());
                    if (int.TryParse(((TextBox)e.EditingElement).Text.ToString(), out int newAmount)
                        && "Amount" == e.Column.Header.ToString()
                        && table.Rows.IndexOf(row) == dataGrid.SelectedIndex)
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
                    table.Rows.RemoveAt(dataGrid.SelectedIndex);
                }
            }
            UpdateTotals();
        }
    }

    public class Receipt
    {
        public Grid grid;

        public Grid CreateGrid(DataTable table, List<string> usedCoupons, Dictionary<string, decimal> discountCodes)
        {
            decimal totalPrice = 0;
            decimal totalDiscount = 0;
            foreach (string coupon in usedCoupons)
            {
                totalDiscount += discountCodes[coupon];
            }
            string usedCouponsString = string.Join(", ", usedCoupons);
            grid = new Grid();
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });

            Label receiptHeadLabel = CreataReceiptLabel("RECEIPT", grid, 0, 0, 20);
            Grid.SetColumnSpan(receiptHeadLabel, 2);

            CreateBackgroundColor(grid, 1, 4);

            Label productNameLabel = CreataReceiptLabel("NAME", grid, 1, 0, 12);

            Label quantityLabel = CreataReceiptLabel("QTY", grid, 1, 1, 12);

            Label priceEachLabel = CreataReceiptLabel("EACH", grid, 1, 2, 12);

            Label totalProductPriceLabel = CreataReceiptLabel("TOTAL", grid, 1, 3, 12);

            StackPanel receiptPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };
            grid.Children.Add(receiptPanel);
            Grid.SetRow(receiptPanel, 2);
            Grid.SetColumn(receiptPanel, 0);
            Grid.SetColumnSpan(receiptPanel, 4);
            //Lägg in produkterna med en foreach och ett nytt grid.
            foreach (DataRow row in table.AsEnumerable())
            {
                receiptPanel.Children.Add(CreateReceiptObjekt(row));
                totalPrice += decimal.Parse(row[1].ToString());
            }

            Label divider = CreataReceiptLabel("======================================================", grid, 3, 0, 12);
            Grid.SetColumnSpan(divider, 4);

            //Label discountCodeLabel = CreataReceiptLabel("Code: ", grid, 4, 0,  12);
            Grid.SetColumnSpan(CreataReceiptLabel("Code: ", grid, 4, 0, 12), 2);

            //Label usedCodeLabel = CreataReceiptLabel("Code", grid, 4, 1,  12);
            Grid.SetColumnSpan(CreataReceiptLabel(usedCouponsString, grid, 4, 1, 12), 2);

            CreateBackgroundColor(grid, 5, 4);

            //Label SumBeforeDiscountLabel = CreataReceiptLabel("SUM: ", grid, 5, 0,  12);
            Grid.SetColumnSpan(CreataReceiptLabel("Total price:", grid, 5, 0, 12), 2);

            Label totalPriceLabel = CreataReceiptLabel($"{totalPrice:N2}kr", grid, 5, 1, 12);
            Grid.SetColumnSpan(totalPriceLabel, 3);

            CreataReceiptLabel("Discount: ", grid, 6, 0, 12);

            string discountedString = $"{totalPrice * totalDiscount:N2}kr ({totalDiscount * 100}%)";
            Label discountLabel = CreataReceiptLabel(discountedString, grid, 6, 1, 12);
            Grid.SetColumnSpan(discountLabel, 3);

            CreateBackgroundColor(grid, 7, 4);

            CreataReceiptLabel("Total Cost: ", grid, 7, 0, 12);

            Label totalPriceDiscountLabel = CreataReceiptLabel($"{totalPrice - (totalPrice * totalDiscount):N2}kr", grid, 7, 1, 12);
            Grid.SetColumnSpan(totalPriceDiscountLabel, 3);

            return grid;
        }

        private Label CreataReceiptLabel(string content, Grid grid, int row, int column, int fontsize)
        {
            Label label = new Label
            {
                Content = content,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = fontsize,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            grid.Children.Add(label);
            Grid.SetRow(label, row);
            Grid.SetColumn(label, column);
            return label;
        }

        private void CreateBackgroundColor(Grid grid, int row, int column)
        {
            Label backGroundColour = new Label
            {
                Background = Brushes.LightGray
            };
            grid.Children.Add(backGroundColour);
            Grid.SetRow(backGroundColour, row);
            Grid.SetColumnSpan(backGroundColour, column);
        }

        //Denna metod ska användas för att bygga ett grid till varje produkt i cart.
        private Grid CreateReceiptObjekt(DataRow row)
        {
            Grid productGrid = new Grid();
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Star) });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });

            decimal unitPrice = decimal.Parse(row[1].ToString()) / decimal.Parse(row[2].ToString());
            CreataReceiptLabel(row[0].ToString(), productGrid, 0, 0, 11);
            CreataReceiptLabel(row[2].ToString(), productGrid, 0, 1, 11);
            CreataReceiptLabel(unitPrice.ToString(), productGrid, 0, 2, 11);
            CreataReceiptLabel(row[1].ToString(), productGrid, 0, 3, 11);

            return productGrid;
        }
    }

    public static class Utility
    {
        public static void CartToCSV(this DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
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

        //FLYTTA OCH FIXA
        public static ImageSource ReadImage(string fileName)
        {
            ImageSource source = new BitmapImage(new Uri(fileName, UriKind.Relative));
            return source;
        }
    }

    public class Product
    {
        public string ArticleName { get; set; }
        public decimal ArticlePrice { get; set; }
        public string ArticleDescription { get; set; }
        public string ImagePath;
    }

    public partial class MainWindow : Window
    {
        public Grid mainGrid;
        public Grid storeGrid;
        public Grid cartGrid;
        public Grid receiptGrid;

        //FLYTTA?
        public List<string[]> file = File.ReadLines(@"Documents\Inventory.csv").Select(a => a.Split(';')).ToList();

        public List<Product> products = new List<Product>();

        //Flytta ner
        public Store myStore = new Store();

        public Cart myCart = new Cart();
        public Receipt myReceipt = new Receipt();

        public Label cartTitle;

        public MainWindow()
        {
            InitializeComponent();
            Start();
            Closed += MainWindow_Closed;
            Closed += CreateLocalFiles;
            //Flytta
            if (MessageBox.Show("Would you like to continue on your last cart?", "Cart", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                myCart.Load();
                myCart.UpdateTotals();
            }
        }


        private void Start()
        {
            CheckLocalStore();
            //Read Cart and business offerings
            ReadOfferings(file, products);
            ReadOfferings(file, myCart.products);
            ReadOfferings(file, myStore.products);

            myCart.ReadDiscountCodes(myCart.discountCoupons, @"Documents\DiscountCodes.csv");
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
            mainGrid = new Grid
            {
                Margin = new Thickness(5)
            };
            root.Content = mainGrid;
            mainGrid.Margin = new Thickness(5);
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());
            mainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Store grid
            storeGrid = myStore.CreateGrid();
            mainGrid.Children.Add(storeGrid);
            storeGrid.Margin = new Thickness(5);
            myStore.addToCartButton.Click += AddToCartButton_Click;

            // Main cart grid
            cartGrid = myCart.CreateGrid();
            mainGrid.Children.Add(cartGrid);
            Grid.SetColumn(cartGrid, 1);
            cartGrid.Margin = new Thickness(5);
            myCart.payButton.Click += PayButton_Click;

            //Ta bort och ersätt med metod
        }
        private void CreateLocalFiles(object sender, EventArgs e)
        {
            if (File.Exists(@"C:\Windows\Temp\JJSTORE"))
            {

            }
            else
            {
                Directory.CreateDirectory(@"C:\Windows\Temp\JJSTORE");
            }
        }

        private void CheckLocalStore()
        {
            if (File.Exists("hejhej"))
            {

            }
        }

        private void ReadOfferings(List<string[]> file, List<Product> products)
        {
            foreach (var line in file)
            {
                Product product = new Product
                {
                    ArticleDescription = line[0],
                    ArticleName = line[1],
                    ArticlePrice = decimal.Parse(line[2])
                };
                products.Add(product);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (myCart.table.Rows.Count > 1)
            {
                myCart.table.CartToCSV(@"C:\Windows\Temp\cart.txt");
            }
        }

        public void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < int.Parse(myStore.storeAmount.Text); i++)
            {
                bool exists = myCart.table.AsEnumerable().Any(row => row.Field<string>("Article Name") == products[myStore.articleList.SelectedIndex].ArticleName);
                if (!exists)
                {
                    DataRow newRow = myCart.table.NewRow();
                    newRow[0] = products[myStore.articleList.SelectedIndex].ArticleName;
                    newRow[1] = products[myStore.articleList.SelectedIndex].ArticlePrice;
                    newRow[2] = 1;
                    newRow[3] = false;
                    myCart.table.Rows.Add(newRow);
                }
                else
                {
                    //Söker och tar fram raden som matchar artikelnamnet, använder first eftersom vi utgår från att det enbart finns en av de namnet och vi vill enbart ha en rad att arbeta med.
                    DataRow result = myCart.table.Select().Where(row => row.Field<string>("Article Name") == products[myStore.articleList.SelectedIndex].ArticleName).First();
                    int newAmount = int.Parse(result[2].ToString()) + 1;
                    result[2] = newAmount;
                    result[1] = newAmount * products[myStore.articleList.SelectedIndex].ArticlePrice;
                }
            }
            myCart.UpdateTotals();
            myStore.storeAmount.Text = "1";
        }

        public void PayButton_Click(object sender, RoutedEventArgs e)
        {
            receiptGrid = myReceipt.CreateGrid(myCart.table, myCart.usedDiscount, myCart.discountCoupons);

            mainGrid.Children.Remove(cartGrid);
            mainGrid.Children.Add(receiptGrid);
            Grid.SetColumn(receiptGrid, 1);
            if (MessageBox.Show("Confirm payment?", "Checkout", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                myCart.table.Clear();
                myCart.UpdateTotals();
                myCart.usedDiscount.Clear();
                MessageBox.Show("Success", "Success", MessageBoxButton.OK);
            }
            mainGrid.Children.Remove(receiptGrid);
            mainGrid.Children.Add(cartGrid);
            Grid.SetColumn(cartGrid, 1);
        }
    }
}