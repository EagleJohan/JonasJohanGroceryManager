using ProductManager;
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
    /// <summary>
    /// Class for handling methods, events and variables related to the Cart
    /// </summary>
    public class Store
    {
        //Variables
        public Image image;

        public ComboBox selection;
        public TextBlock title;
        public TextBlock description;
        public TextBox quantity;
        public Label price;
        public Button addProduct;
        public Button showProductManager;

        public List<Product> products = new List<Product>();

        //Methods

        /// <summary>
        /// Creates grid for the store
        /// </summary>
        /// <returns></returns>
        public Grid CreateGrid()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { });
            grid.RowDefinitions.Add(new RowDefinition { MaxHeight = 50 });
            grid.RowDefinitions.Add(new RowDefinition { });

            Label storeTitle = new Label
            {
                Content = "Little Shop of Greens",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = Brushes.DarkGray,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            grid.Children.Add(storeTitle);

            WrapPanel wrapPanel = new WrapPanel
            {
                Orientation = Orientation.Horizontal
            };
            grid.Children.Add(wrapPanel);
            Grid.SetRow(wrapPanel, 1);
            //Start image, before any product has been chosen
            image = new Image
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5),
                Stretch = Stretch.UniformToFill,
                Width = 250,
                Height = 250,
                Source = Utility.ReadImage(@"Pictures\Placeholder2.png")
            };
            wrapPanel.Children.Add(image);

            //Grid for both choosing articles and description
            Grid articleGrid = new Grid();
            articleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Star) });
            articleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Star) });
            articleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(65, GridUnitType.Star) });
            articleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10, GridUnitType.Star) });
            articleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(15, GridUnitType.Star) });
            articleGrid.ColumnDefinitions.Add(new ColumnDefinition());
            wrapPanel.Children.Add(articleGrid);

            showProductManager = new Button
            {
                Content = "Manage Store",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            wrapPanel.Children.Add(showProductManager);

            //Combobox to choose article
            selection = new ComboBox
            {
                Name = "Articles",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                ItemsSource = products.Select(products => products.ArticleName),
                MaxWidth = 200
            };
            selection.DropDownOpened += ArticleList_DropDownOpened;
            selection.SelectionChanged += ArticleList_SelectionChanged;
            articleGrid.Children.Add(selection);

            //Header over article list
            title = new TextBlock
            {
                Text = "Article",
                IsHitTestVisible = false,
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold
            };
            articleGrid.Children.Add(title);

            //Label to describe the chosen article
            description = new TextBlock
            {
                Text = "Description of articles",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 200,
                FontSize = 12,
                FontWeight = FontWeights.SemiBold,
                FontStyle = FontStyles.Italic
            };
            articleGrid.Children.Add(description);
            Grid.SetRow(description, 1);

            price = new Label
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "Price:",
                FontSize = 15,
                FontWeight = FontWeights.Bold,
            };
            articleGrid.Children.Add(price);
            Grid.SetRow(price, 2);

            //Grid for adding articles to cart
            Grid addProductGrid = new Grid();
            articleGrid.Children.Add(addProductGrid);
            Grid.SetRow(addProductGrid, 4);
            addProductGrid.RowDefinitions.Add(new RowDefinition { });
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) });
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60, GridUnitType.Star) });
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) });
            addProductGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });

            //Amount to add to cart, default is one
            quantity = new TextBox
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 15,
                Text = "1",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            quantity.TextChanged += CheckForMinimumQuantity;
            quantity.GotFocus += SelectionStartQuantity;
            quantity.LostFocus += CheckQuantity;
            addProductGrid.Children.Add(quantity);
            quantity.KeyDown += Integers_KeyDown;
            Grid.SetColumn(quantity, 1);
            //Button to decrease amount
            Button decreaseQuantity = new Button
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "-",
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
            };
            decreaseQuantity.Click += DecreaseQuantity_Click;
            addProductGrid.Children.Add(decreaseQuantity);
            //button to increase amount
            Button increaseQuantity = new Button
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "+",
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
            };
            increaseQuantity.Click += IncreaseQuantity_Click;
            addProductGrid.Children.Add(increaseQuantity);
            Grid.SetColumn(increaseQuantity, 2);

            //button to add to cart
            addProduct = new Button
            {
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                Content = "Add to cart",
                IsEnabled = false,
                FontSize = 12,
                FontWeight = FontWeights.Bold
            };
            addProductGrid.Children.Add(addProduct);
            Grid.SetColumn(addProduct, 3);
            return grid;
        }

        //Events

        /// <summary>
        /// Makes sure quantity textbox is never empty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckQuantity(object sender, RoutedEventArgs e)
        {
            if (quantity.Text.Length == 0)
            {
                quantity.Text = "1";
            }
        }

        /// <summary>
        /// Displays correct information and image when user chooses product in combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArticleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            image.Source = Utility.ReadImage(@"C:\Windows\Temp\JJSTORE\Pictures\" + products[selection.SelectedIndex].ImagePath);
            price.Content = "Price: " + products[selection.SelectedIndex].ArticlePrice + " SEK";
            description.Text = products[selection.SelectedIndex].ArticleDescription;
            addProduct.IsEnabled = true;
        }

        /// <summary>
        /// Removes header when articlelist combobox is opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ArticleList_DropDownOpened(object sender, EventArgs e)
        {
            title.Text = "";
        }

        /// <summary>
        /// Aligns selection to the right
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionStartQuantity(object sender, RoutedEventArgs e)
        {
            quantity.SelectionStart = quantity.Text.Length;
            quantity.SelectionLength = 0;
        }

        /// <summary>
        /// Sets value in quantity textbox to 1 if the value is lower than 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckForMinimumQuantity(object sender, TextChangedEventArgs e)
        {
            int.TryParse(quantity.Text, out int currentQuantity);
            if (quantity.Text.Length > 0 && currentQuantity < 1)
            {
                quantity.Text = "1";
            }
        }

        /// <summary>
        /// Increases value in quantity textbox by 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(quantity.Text, out int currentQuantity);
            currentQuantity++;
            quantity.Text = currentQuantity.ToString();
        }

        /// <summary>
        /// Decreases value in quantity textbox by 1 but never to 0
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(quantity.Text, out int currentQuantity);
            if (currentQuantity < 1)
            {
                quantity.Text = "1";
            }
            else
            {
                currentQuantity--;
                quantity.Text = currentQuantity.ToString();
            }
        }

        /// <summary>
        /// Does not allow user to enter anyother key but numbers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Integers_KeyDown(object sender, KeyEventArgs e)
        {
            var digitkeys = e.Key >= Key.D0 && e.Key <= Key.D9;
            var numpadKeys = e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9;
            var modifiedKey = e.KeyboardDevice.Modifiers == ModifierKeys.None;
            if (modifiedKey && (digitkeys || numpadKeys))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }

    /// <summary>
    /// Class for handling methods, events and variables related to the Cart
    /// </summary>
    public class Cart
    {
        //Varibles
        public DataColumn quantity;

        public DataColumn delete;
        public TextBox discountCode;
        public DataTable table;
        public DataGrid dataGrid;
        public Grid grid;
        public Label totals;
        public int totalQuantity = 0;
        public decimal totalPrice = 0;
        public Dictionary<string, decimal> discountCoupons = new Dictionary<string, decimal>();
        public List<string> usedDiscount = new List<string>();
        public List<Product> products = new List<Product>();
        public Button pay;

        //Methods

        /// <summary>
        /// Reads and adds all avaiable discount codes to a list
        /// </summary>
        /// <param name="discountCodes"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public Dictionary<string, decimal> ReadDiscountCodes(Dictionary<string, decimal> discountCodes, string filePath)
        {
            var lines = File.ReadLines(filePath).Select(a => a.Split(';')).ToList();
            foreach (var line in lines)
            {
                discountCodes.Add(line[0], decimal.Parse(line[1]));
            }
            return discountCodes;
        }


        /// <summary>
        /// Creates the main cart grid
        /// </summary>
        /// <returns></returns>
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
                Content = "Your Shopping Cart",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = Brushes.DarkGray,
                VerticalAlignment = VerticalAlignment.Center
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

        /// <summary>
        /// Creates the datagrid and table where added products are shown
        /// </summary>
        /// <returns></returns>
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
                CanUserSortColumns = false,
                Margin = new Thickness(5),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
            };

            grid.CellEditEnding += Grid_CellEditEnding;
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
            quantity = new DataColumn
            {
                ColumnName = "Qty",
                DataType = typeof(int)
            };
            table.Columns.Add(quantity);
            delete = new DataColumn
            {
                ColumnName = "Delete",
                DataType = typeof(bool)
            };
            table.Columns.Add(delete);
            grid.ItemsSource = table.DefaultView;
            grid.Loaded += Grid_Loaded;
            return grid;
        }

        /// <summary>
        /// Sets width of the datatable rows using proportions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.Columns[0].Width = new DataGridLength(50, DataGridLengthUnitType.Star);
            dataGrid.Columns[1].Width = new DataGridLength(20, DataGridLengthUnitType.Star);
            dataGrid.Columns[2].Width = new DataGridLength(15, DataGridLengthUnitType.Star);
            dataGrid.Columns[3].Width = new DataGridLength(15, DataGridLengthUnitType.Star);
        }

        /// <summary>
        /// Creates the grid containing discounts and checkoutbuttons
        /// </summary>
        /// <returns></returns>
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
            totals = new Label
            {
                Content = "Totals",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 15,
                FontWeight = FontWeights.SemiBold
            };
            grid.Children.Add(totals);
            Grid.SetColumnSpan(totals, 5);

            //discount label
            Label coupon = new Label
            {
                Content = "Coupon:",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
            };
            grid.Children.Add(coupon);
            Grid.SetRow(coupon, 1);
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
                Padding = new Thickness(5),
                FontSize = 15,
                FontWeight = FontWeights.Bold,
            };
            grid.Children.Add(addDiscountCode);
            addDiscountCode.Click += AddDiscountCode;
            Grid.SetColumn(addDiscountCode, 2);
            Grid.SetRow(addDiscountCode, 1);

            // Print receipt and pay for cart
            pay = new Button
            {
                Content = "Checkout",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Green
            };
            grid.Children.Add(pay);
            Grid.SetColumn(pay, 3);
            Grid.SetRow(pay, 1);

            //Clear the whole cart
            Button clear = new Button
            {
                Content = "Clear Cart",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Red
            };
            clear.Click += delegate { table.Rows.Clear(); };
            grid.Children.Add(clear);
            Grid.SetColumn(clear, 4);
            Grid.SetRow(clear, 1);
            return grid;
        }

        /// <summary>
        /// Calculates as total sum for price and quantity by adding all rows in table
        /// </summary>
        public void UpdateTotals()
        {
            totalQuantity = 0;
            totalPrice = 0;
            decimal totalDiscount = 0.0M;
            foreach (DataRow row in table.AsEnumerable())
            {
                totalQuantity += int.Parse(row[2].ToString());
                totalPrice += decimal.Parse(row[1].ToString());
            }
            foreach (string coupon in usedDiscount)
            {
                totalDiscount += discountCoupons[coupon];
            }
            totals.Content = $"Total quantity: {totalQuantity}pcs  Total price: {totalPrice:N2}kr\n" +
                                 $"Total price after discount coupons: {totalPrice * (1 - totalDiscount):N2}kr";
        }

        /// <summary>
        /// Loads the last cart used (if there is one) if the user chooses that option at the start of the programm
        /// </summary>
        public void Load()
        {
            if (File.Exists(@"C:\Windows\Temp\JJSTORE\cart.csv"))
            {
                //populates table with saved cart data
                List<string[]> lines = File.ReadLines(@"C:\Windows\Temp\JJSTORE\cart.csv").Select(a => a.Split(';')).ToList();
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

        /// <summary>
        /// Checks entered discountcode for validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddDiscountCode(object sender, RoutedEventArgs e)
        {
            string inputDiscount = discountCode.Text.ToLower();
            if (discountCoupons.ContainsKey(inputDiscount) && !usedDiscount.Contains(inputDiscount))
            {
                usedDiscount.Add(inputDiscount);

                discountCode.BorderBrush = Brushes.Black;
            }
            else if (discountCoupons.ContainsKey(inputDiscount) && usedDiscount.Contains(inputDiscount))
            {
                MessageBox.Show("Coupon is already in use", "Coupon", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                discountCode.BorderBrush = Brushes.Red;
            }
            discountCode.Text = "";
            UpdateTotals();
        }

        /// <summary>
        /// Lets user change quantity in cart(table) and corrects price accordingly. Also stops user from tampering with price.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if ("Qty" == e.Column.Header.ToString() || "Price" == e.Column.Header.ToString())
            {
                totalQuantity = 0;
                totalPrice = 0;
                //Check if the price matches productprice and if checkbox isDeleted is checked
                foreach (var row in table.AsEnumerable())
                {
                    int correctQuantity = int.Parse(row[2].ToString());
                    if (int.TryParse(((TextBox)e.EditingElement).Text.ToString(), out int newQuantity)
                        && "Qty" == e.Column.Header.ToString()
                        && table.Rows.IndexOf(row) == dataGrid.SelectedIndex)
                    {
                        correctQuantity = newQuantity;
                    }
                    var productNames = products.Select(products => products.ArticleName).ToList();
                    int indexOfProduct = productNames.IndexOf(row[0].ToString());
                    row[2] = correctQuantity;
                    row[1] = correctQuantity * products[indexOfProduct].ArticlePrice;
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

    /// <summary>
    /// Class for handling methods, events and variables related to the Receipt
    /// </summary>
    public class Receipt
    {
        //Variables
        public Grid grid;
        public Button pay;
        public Button cancel;

        /// <summary>
        /// Creates  main grid for receipt
        /// </summary>
        /// <param name="table"></param>
        /// <param name="usedCoupons"></param>
        /// <param name="discountCodes"></param>
        /// <returns></returns>
        public Grid CreateGrid(DataTable table, List<string> usedCoupons, Dictionary<string, decimal> discountCodes)
        {
            decimal totalPrice = 0;
            decimal totalDiscount = 0;
            //Checks which coupons have been used
            foreach (string coupon in usedCoupons)
            {
                totalDiscount += discountCodes[coupon];
            }
            string usedCouponsString = string.Join(", ", usedCoupons);

            //Receipt main grid
            grid = new Grid();
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });

            Label receiptHeadLabel = CreateLabel("RECEIPT", grid, 0, 0, 20);
            Grid.SetColumnSpan(receiptHeadLabel, 2);

            CreateBackgroundColor(grid, 1, 4);

            Label productNameLabel = CreateLabel("NAME", grid, 1, 0, 12);

            Label quantityLabel = CreateLabel("QTY", grid, 1, 1, 12);

            Label priceEachLabel = CreateLabel("EACH", grid, 1, 2, 12);

            Label totalProductPriceLabel = CreateLabel("TOTAL", grid, 1, 3, 12);

            StackPanel receiptPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };
            grid.Children.Add(receiptPanel);
            Grid.SetRow(receiptPanel, 2);
            Grid.SetColumn(receiptPanel, 0);
            Grid.SetColumnSpan(receiptPanel, 4);
            //Adds every product in cart to its own grid which is then added to a stackpanel in main grid
            foreach (DataRow row in table.AsEnumerable())
            {
                receiptPanel.Children.Add(CreateReceiptObjekt(row));
                totalPrice += decimal.Parse(row[1].ToString());
            }

            Label divider = CreateLabel("======================================================", grid, 3, 0, 12);
            Grid.SetColumnSpan(divider, 4);

            Grid.SetColumnSpan(CreateLabel("Code: ", grid, 4, 0, 12), 2);

            Grid.SetColumnSpan(CreateLabel(usedCouponsString, grid, 4, 1, 12), 2);

            CreateBackgroundColor(grid, 5, 4);

            Grid.SetColumnSpan(CreateLabel("Total price:", grid, 5, 0, 12), 2);

            Label totalPriceLabel = CreateLabel($"{totalPrice:N2}kr", grid, 5, 1, 12);
            Grid.SetColumnSpan(totalPriceLabel, 3);

            CreateLabel("Discount: ", grid, 6, 0, 12);

            string discountedString = $"{totalPrice * totalDiscount:N2}kr ({totalDiscount * 100}%)";
            Label discountLabel = CreateLabel(discountedString, grid, 6, 1, 12);
            Grid.SetColumnSpan(discountLabel, 3);

            CreateBackgroundColor(grid, 7, 4);

            CreateLabel("Total Cost: ", grid, 7, 0, 12);

            Label totalPriceDiscountLabel = CreateLabel($"{totalPrice - (totalPrice * totalDiscount):N2}kr", grid, 7, 1, 12);
            Grid.SetColumnSpan(totalPriceDiscountLabel, 3);

            Label confirmPayment = new Label
            {
                Content = "Are you sure?",
                Margin = new Thickness(5),
                Padding = new Thickness(5),
                FontWeight = FontWeights.SemiBold,
                FontSize = 15
            };
            grid.Children.Add(confirmPayment);
            Grid.SetRow(confirmPayment, 8);
            Grid.SetColumnSpan(confirmPayment, 2);

            pay = new Button
            {
                Content = "Pay",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            grid.Children.Add(pay);
            Grid.SetColumn(pay, 2);
            Grid.SetRow(pay, 8);

            cancel = new Button
            {
                Content = "Cancel",
                Margin = new Thickness(5),
                Padding = new Thickness(5)
            };
            grid.Children.Add(cancel);
            Grid.SetColumn(cancel, 3);
            Grid.SetRow(cancel, 8);

            return grid;
        }

        /// <summary>
        /// Template for similar labels
        /// </summary>
        /// <param name="content"></param>
        /// <param name="grid"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="fontsize"></param>
        /// <returns></returns>
        private Label CreateLabel(string content, Grid grid, int row, int column, int fontsize)
        {
            Label label = new Label
            {
                Content = content,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 15,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            grid.Children.Add(label);
            Grid.SetRow(label, row);
            Grid.SetColumn(label, column);
            return label;
        }

        /// <summary>
        /// Adds background color to labels made from template
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private void CreateBackgroundColor(Grid grid, int row, int column)
        {
            Label backgroundColour = new Label
            {
                Background = Brushes.LightGray
            };
            grid.Children.Add(backgroundColour);
            Grid.SetRow(backgroundColour, row);
            Grid.SetColumnSpan(backgroundColour, column);
        }

       /// <summary>
       /// Creates a grid for each product in cart
       /// </summary>
       /// <param name="row"></param>
       /// <returns></returns>
        private Grid CreateReceiptObjekt(DataRow row)
        {
            Grid productGrid = new Grid();
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Star) });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star) });

            decimal unitPrice = decimal.Parse(row[1].ToString()) / decimal.Parse(row[2].ToString());
            CreateLabel(row[0].ToString(), productGrid, 0, 0, 11);
            CreateLabel(row[2].ToString(), productGrid, 0, 1, 11);
            CreateLabel(unitPrice.ToString(), productGrid, 0, 2, 11);
            CreateLabel(row[1].ToString(), productGrid, 0, 3, 11);

            return productGrid;
        }
    }

    /// <summary>
    /// Class for handling methods, events and variables related to the IO-actions
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Writes current cart to Temp when program is closed
        /// </summary>
        /// <param name="datatable"></param>
        /// <param name="filepath"></param>
        public static void CartToCSV(this DataTable datatable, string filepath)
        {
            StreamWriter sw = new StreamWriter(filepath, false);
            foreach (DataRow row in datatable.Rows)
            {
                for (int i = 0; i < datatable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(row[i]))
                    {
                        sw.Write(row[i].ToString());
                    }
                    if (i < datatable.Columns.Count - 1)
                    {
                        sw.Write(";");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        /// <summary>
        /// Creates an image source
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ImageSource ReadImage(string fileName)
        {
            ImageSource source = new BitmapImage(new Uri(fileName, UriKind.RelativeOrAbsolute));
            return source;
        }
    }

    /// <summary>
    /// Class for creating products
    /// </summary>
    public class Product
    {
        public string ArticleName { get; set; }
        public decimal ArticlePrice { get; set; }
        public string ArticleDescription { get; set; }
        public string ImagePath { get; set; }
    }

    public partial class MainWindow : Window
    {
        //Grids for all sections
        public Grid mainGrid;
        public Grid storeGrid;
        public Grid cartGrid;
        public Grid receiptGrid;

        //File paths to Temp
        public string discountCodePath = @"C:\Windows\Temp\JJSTORE\Documents\DiscountCodes.csv";
        public string inventoryPath = @"C:\Windows\Temp\JJSTORE\Documents\Inventory.csv";
        public string pictureDirectoryPath = @"C:\Windows\Temp\JJSTORE\Pictures\";

        public List<Product> products = new List<Product>();

        //Instance variables - classes
        public Store myStore = new Store();
        public Cart myCart = new Cart();
        public Receipt myReceipt = new Receipt();
        //Instance variables - projects
        private ManagerWindow managerWindow = new ManagerWindow();

        public MainWindow()
        {
            InitializeComponent();
            Start();
            //Saves the current cart when app is closed automatically
            Closed += MainWindow_Closed;
            //Loads the last cart if the user wishes it, and if there is one
            if (MessageBox.Show("Would you like to continue on your last cart?", "Cart", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                myCart.Load();
                myCart.UpdateTotals();
            }
        }

        private void Start()
        {
            //Find all paths
            LocatePaths();
            LoadLocalFiles();
            // Window options
            Title = "Generic Store AB";
            SizeToContent = SizeToContent.Height;
            Width = 1000;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Scrolling
            ScrollViewer root = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                CanContentScroll = true
            };
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
            myStore.addProduct.Click += AddToCartButton_Click;
            myStore.showProductManager.Click += ProductManager_Click;

            // Main cart grid
            cartGrid = myCart.CreateGrid();
            mainGrid.Children.Add(cartGrid);
            Grid.SetColumn(cartGrid, 1);
            cartGrid.Margin = new Thickness(5);
            myCart.pay.Click += PayButton_Click;
        }

        /// <summary>
        /// Loads and updates inventory and discount codes from Temp, clears first to avoid duplicates
        /// </summary>
        private void LoadLocalFiles()
        {
            //Reads offerings 
            products.Clear();
            ReadOfferings(File.ReadLines(inventoryPath).Select(a => a.Split(';')).ToList(), products);
            myCart.products.Clear();
            ReadOfferings(File.ReadLines(inventoryPath).Select(a => a.Split(';')).ToList(), myCart.products);
            myStore.products.Clear();
            ReadOfferings(File.ReadLines(inventoryPath).Select(a => a.Split(';')).ToList(), myStore.products);
            //Reads discountcodes
            myCart.discountCoupons.Clear();
            myCart.usedDiscount.Clear();
            myCart.ReadDiscountCodes(myCart.discountCoupons, discountCodePath);
        }

        /// <summary>
        /// Opens managerwindow, updates files both at submitbuttonclick and at closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductManager_Click(object sender, RoutedEventArgs e)
        {
            managerWindow = new ManagerWindow();
            managerWindow.submit.Click += (sender, e) => LoadLocalFiles();
            managerWindow.submit.Click += delegate { myStore.selection.ItemsSource = products.Select(products => products.ArticleName); };
            managerWindow.Closing += (sender, e) => LoadLocalFiles();
            managerWindow.Closing += delegate { myStore.selection.ItemsSource = products.Select(products => products.ArticleName); };
            managerWindow.Show();
        }

        /// <summary>
        /// Creates directory in Temp and copies files from solution
        /// </summary>
        private void LocatePaths()
        {
            //If file exists do nothing
            if (!File.Exists(@"C:\Windows\Temp\JJSTORE\Documents\Inventory.csv"))
            {
                //Reads document from project and copies to local destination
                var documentFiles = Directory.GetFiles(@"Documents\");
                Directory.CreateDirectory(@"C:\Windows\Temp\JJSTORE\Documents");
                foreach (var file in documentFiles)
                {
                    File.Copy(file, @"C:\Windows\Temp\JJSTORE\" + file);
                }
                //Reads images from project and copies to local destination
                var imageFiles = Directory.GetFiles(@"Pictures\");
                Directory.CreateDirectory(@"C:\Windows\Temp\JJSTORE\Pictures");
                foreach (var file in imageFiles)
                {
                    File.Copy(file, @"C:\Windows\Temp\JJSTORE\" + file);
                }
            }
        }

        /// <summary>
        /// Creates list of products
        /// </summary>
        /// <param name="file"></param>
        /// <param name="products"></param>
        public static void ReadOfferings(List<string[]> file, List<Product> products)
        {
            foreach (var line in file)
            {
                Product product = new Product
                {
                    ArticleDescription = line[0],
                    ArticleName = line[1],
                    ArticlePrice = decimal.Parse(line[2]),
                    ImagePath = line[3]
                };
                products.Add(product);
            }
        }

        /// <summary>
        /// Saves current cart, makes sure the whole program closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (myCart.table.Rows.Count > 0)
            {
                myCart.table.CartToCSV(@"C:\Windows\Temp\JJSTORE\cart.csv");
            }
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Creates new row in datatable and adds data from selected product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < int.Parse(myStore.quantity.Text); i++)
            {
                bool exists = myCart.table.AsEnumerable().Any(row => row.Field<string>("Article Name") == products[myStore.selection.SelectedIndex].ArticleName);
                if (!exists)
                {
                    DataRow newRow = myCart.table.NewRow();
                    newRow[0] = products[myStore.selection.SelectedIndex].ArticleName;
                    newRow[1] = products[myStore.selection.SelectedIndex].ArticlePrice;
                    newRow[2] = 1;
                    newRow[3] = false;
                    myCart.table.Rows.Add(newRow);
                }
                else
                {
                    //Locates the datatablerow that matches the articlename. Using first on assumption that there is only one instance of that particular name and we only want one row to work with.
                    DataRow result = myCart.table.Select().Where(row => row.Field<string>("Article Name") == products[myStore.selection.SelectedIndex].ArticleName).First();
                    int newQuantity = int.Parse(result[2].ToString()) + 1;
                    result[2] = newQuantity;
                    result[1] = newQuantity * products[myStore.selection.SelectedIndex].ArticlePrice;
                }
            }
            myCart.UpdateTotals();
            //Changes quantity back to 1
            myStore.quantity.Text = "1";
        }

        /// <summary>
        /// If there are products in cart; removes cartgrid and replaces with receiptgrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PayButton_Click(object sender, RoutedEventArgs e)
        {
            //Check if there are rows in datatable (products in cart)
            if (myCart.table.Rows.Count > 0)
            {
                receiptGrid = myReceipt.CreateGrid(myCart.table, myCart.usedDiscount, myCart.discountCoupons);

                mainGrid.Children.Remove(cartGrid);
                mainGrid.Children.Add(receiptGrid);
                Grid.SetColumn(receiptGrid, 1);

                myReceipt.pay.Click += SuccesfulPayment;
                myReceipt.cancel.Click += AbortPayment;


                //Disables add to cart button while receipt is showing
                myStore.addProduct.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Cart is empty");
            }

        }

        /// <summary>
        /// Removes receiptgrid and brings back the cartgrid with the current cart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AbortPayment(object sender, RoutedEventArgs e)
        {
            mainGrid.Children.Remove(receiptGrid);
            mainGrid.Children.Add(cartGrid);
            Grid.SetColumn(cartGrid, 1);
            //turns the add to cart button on again
            myStore.addProduct.IsEnabled = true;
        }

        /// <summary>
        /// Clears cart, resets discounts and removes receiptgrid and brings back the cartgrid with an empty cart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SuccesfulPayment(object sender, RoutedEventArgs e)
        {
            myCart.table.Clear();
            myCart.UpdateTotals();
            myCart.usedDiscount.Clear();
            MessageBox.Show("Success", "Success", MessageBoxButton.OK);
            mainGrid.Children.Remove(receiptGrid);
            mainGrid.Children.Add(cartGrid);
            Grid.SetColumn(cartGrid, 1);

            //turns the add to cart button on again
            myStore.addProduct.IsEnabled = true;
        }
    }
}