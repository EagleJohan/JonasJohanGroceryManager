using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProductManager
{
    public class Discount
    {
        //Variables
        public Label discountKeyLabel;

        public TextBox discountKeyBox;
        public Label discountValueLabel;
        public TextBox discountValueBox;
        public ListBox discountCodeList;

        public Dictionary<string, decimal> discountCodes = new Dictionary<string, decimal>();

        public string filePath = @"C:\Windows\Temp\JJSTORE\Documents\DiscountCodes.csv";

        /// <summary>
        /// Creates and shows discount grid
        /// </summary>
        /// <returns></returns>
        public Grid ShowDiscountGrid()
        {
            ReadDiscountCodes(filePath);
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            discountCodeList = new ListBox
            {
                MinHeight = 200,
                Margin = new Thickness(5)
            };
            UpdateListBox();
            grid.Children.Add(discountCodeList);
            Grid.SetColumnSpan(discountCodeList, 4);

            Button deleteDiscount = new Button
            {
                Content = "Delete",
                Margin = new Thickness(5),
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                MaxWidth = 100,
                MaxHeight = 25,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            deleteDiscount.Click += DeleteDiscount;
            grid.Children.Add(deleteDiscount);
            Grid.SetRow(deleteDiscount, 1);
            Grid.SetColumn(deleteDiscount, 0);
            Grid.SetColumnSpan(deleteDiscount, 4);

            discountKeyLabel = new Label
            {
                Content = "Code",
                Margin = new Thickness(5, 5, 0, 5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            grid.Children.Add(discountKeyLabel);
            Grid.SetRow(discountKeyLabel, 2);
            Grid.SetColumn(discountKeyLabel, 0);

            discountKeyBox = new TextBox
            {
                Width = 150,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxLength = 30
            };
            grid.Children.Add(discountKeyBox);
            Grid.SetRow(discountKeyBox, 2);
            Grid.SetColumn(discountKeyBox, 1);

            discountValueLabel = new Label
            {
                Content = "Value",
                Margin = new Thickness(5, 5, 0, 5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            grid.Children.Add(discountValueLabel);
            Grid.SetRow(discountValueLabel, 2);
            Grid.SetColumn(discountValueLabel, 2);

            discountValueBox = new TextBox
            {
                Text = "0",
                Width = 150,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxLength = 2
            };
            discountValueBox.KeyDown += Integers_KeyDown;
            grid.Children.Add(discountValueBox);
            Grid.SetRow(discountValueBox, 2);
            Grid.SetColumn(discountValueBox, 3);
            return grid;
        }

        /// <summary>
        /// Deletes selected code
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteDiscount(object sender, RoutedEventArgs e)
        {
            if (discountCodeList.SelectedIndex >= 0)
            {
                string keyToDelete = discountCodeList.SelectedItem.ToString();

                discountCodes.Remove(keyToDelete);
            }

            UpdateListBox();
            UpdateCSVFile(filePath);
        }

        /// <summary>
        /// Reads all existing codes from Temp and add to a dictionary
        /// </summary>
        /// <param name="filePath"></param>
        public void ReadDiscountCodes(string filePath)
        {
            var lines = File.ReadLines(filePath).Select(a => a.Split(';')).ToList();
            discountCodes.Clear();
            foreach (var line in lines)
            {
                discountCodes.Add(line[0], decimal.Parse(line[1]));
            }
        }

        /// <summary>
        /// Clears listbox to avoid duplicates then updates with latest list
        /// </summary>
        public void UpdateListBox()
        {
            discountCodeList.Items.Clear();
            foreach (var code in discountCodes)
            {
                discountCodeList.Items.Add(code.Key);
            }
        }

        /// <summary>
        /// Method for only handling digits and comma keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Integers_KeyDown(object sender, KeyEventArgs e)
        {
            var digitkeys = e.Key >= Key.D0 && e.Key <= Key.D9;
            var numpadKeys = e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9;
            var modifiedKey = e.KeyboardDevice.Modifiers == ModifierKeys.None;
            if ((modifiedKey && (digitkeys || numpadKeys)) || (e.Key == Key.OemComma))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Overwrites CSV in Temp with updated information
        /// </summary>
        /// <param name="filePath"></param>
        public void UpdateCSVFile(string filePath)
        {
            StreamWriter sw = new StreamWriter(filePath, false);
            foreach (var code in discountCodes)
            {
                sw.WriteLine($"{code.Key};{code.Value}");
            }
            sw.Close();
        }
    }

    public class ItemAdd
    {
        ///Variables
        public TextBox name;

        public TextBox description;
        public TextBox price;
        public Button imageSelect;
        public Label imageURLLabel;
        public string imageURL;

        public List<string> currentProducts = new List<string>();

        public OpenFileDialog fileDialog = new OpenFileDialog();

        /// <summary>
        /// Creates and shows add product grid
        /// </summary>
        public Grid ShowItemAdd()
        {
            Grid grid = new Grid();
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
            grid.Children.Add(stackPanel);

            Label articleLabel = new Label
            {
                Content = "Article name",
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(articleLabel);

            name = new TextBox
            {
                Name = "",
                Width = 150,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                MaxLength = 30
            };
            stackPanel.Children.Add(name);

            Label descriptionLabel = new Label
            {
                Content = "Description",
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(descriptionLabel);

            description = new TextBox
            {
                Name = "",
                Width = 150,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                MaxLength = 200
            };
            stackPanel.Children.Add(description);

            Label priceLabel = new Label
            {
                Content = "Price",
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(priceLabel);

            price = new TextBox
            {
                Name = "",
                Width = 150,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                MaxLength = 10
            };
            price.KeyDown += Integers_KeyDown;
            stackPanel.Children.Add(price);

            imageURLLabel = new Label
            {
                Content = "",
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(imageURLLabel);
            imageSelect = new Button
            {
                Content = "Add image",
                Margin = new Thickness(5),
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                MaxWidth = 100,
                MaxHeight = 25,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            //Lets user search for an image locally
            imageSelect.Click += OpenDialog;
            stackPanel.Children.Add(imageSelect);

            currentProducts = ReadInventory(File.ReadLines(@"C:\Windows\Temp\JJSTORE\Documents\Inventory.csv").Select(a => a.Split(';')).ToList());

            return grid;
        }

        /// <summary>
        /// Reads inventory list from file
        /// </summary>
        /// <param name="file"></param>
        public List<string> ReadInventory(List<string[]> file)
        {
            List<string> products = new List<string>();
            foreach (var line in file)
            {
                products.Add(line[1].ToLower());
            }

            return products;
        }

        /// <summary>
        /// Method for only handling digits and comma keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Integers_KeyDown(object sender, KeyEventArgs e)
        {
            var digitkeys = e.Key >= Key.D0 && e.Key <= Key.D9;
            var numpadKeys = e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9;
            var modifiedKey = e.KeyboardDevice.Modifiers == ModifierKeys.None;
            if ((modifiedKey && (digitkeys || numpadKeys)) || (e.Key == Key.OemComma))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Lets user search for images and show path of chosen in imageURLLabel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDialog(object sender, RoutedEventArgs e)
        {
            bool? isOK = true;
            if (isOK == fileDialog.ShowDialog())
            {
                imageURL = fileDialog.FileName;
                imageURLLabel.Content = $"Image url: \n{imageURL}";
            }
        }
    }

    public class ItemRemove
    {
        //Variables
        public ListBox productList;

        public string filePath = @"C:\Windows\Temp\JJSTORE\Documents\Inventory.csv";
        public List<string[]> file;

        /// <summary>
        /// Creates and shows delete product grid
        /// </summary>
        /// <returns></returns>
        public Grid ShowItemHandler()
        {
            file = File.ReadLines(filePath).Select(a => a.Split(';')).ToList();
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            productList = new ListBox
            {
                MinHeight = 200,
                Margin = new Thickness(5)
            };
            UpdateListBox();
            grid.Children.Add(productList);

            Button delete = new Button
            {
                Content = "Delete",
                Margin = new Thickness(5),
                Width = 130,
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                MaxWidth = 100,
                MaxHeight = 25
            };
            delete.Click += Delete_Click;
            grid.Children.Add(delete);
            Grid.SetRow(delete, 1);

            return grid;
        }

        /// <summary>
        /// Clears product list to avoid duplicates and then updates list from Temp
        /// </summary>
        private void UpdateListBox()
        {
            productList.Items.Clear();
            foreach (var product in file)
            {
                productList.Items.Add(product[1]);
            }
        }

        /// <summary>
        /// Deletes selected product
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (productList.SelectedIndex >= 0)
            {
                if (File.Exists(@"C:\Windows\Temp\JJSTORE\Pictures\" + file[productList.SelectedIndex][3]))
                {
                    File.Delete(@"C:\Windows\Temp\JJSTORE\Pictures\" + file[productList.SelectedIndex][3]);
                }
                file.RemoveAt(productList.SelectedIndex);
                UpdateListBox();
                UpdateCSVFile(filePath);
            }
            else
            {
                MessageBox.Show("Please select a product to delete");
            }
        }

        /// <summary>
        /// Overwrites CSV in Temp with latest information
        /// </summary>
        /// <param name="filePath"></param>
        public void UpdateCSVFile(string filePath)
        {
            StreamWriter sw = new StreamWriter(filePath, false);
            foreach (var product in file)
            {
                sw.WriteLine($"{product[0]};{product[1]};{product[2]};{product[3]}");
            }
            sw.Close();
        }
    }

    public partial class ManagerWindow : Window
    {
        //Variables
        public Grid grid;

        private ComboBox manageSelection;

        private ItemAdd itemAdd = new ItemAdd();
        private Grid addGrid;

        private ItemRemove itemRemove = new ItemRemove();
        private Grid removeGrid;

        public Discount discount = new Discount();
        public Grid discountGrid;

        public Button submit;

        public ManagerWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "Manage Products";
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            SizeToContent = SizeToContent.Height;
            MinHeight = 500;
            Width = 430;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            grid = new Grid();
            root.Content = grid;
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition { MaxHeight = 50 });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(20, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            Label title = new Label
            {
                Content = "Product manager!",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5),
                FontSize = 20,
                FontWeight = FontWeights.SemiBold
            };
            grid.Children.Add(title);

            manageSelection = new ComboBox
            {
                Margin = new Thickness(5),
                MaxHeight = 20
            };
            manageSelection.Items.Add("Add new item");
            manageSelection.Items.Add("Delete items");
            manageSelection.Items.Add("Manage discounts");
            manageSelection.SelectionChanged += ManageSelection_SelectionChanged;
            grid.Children.Add(manageSelection);
            Grid.SetRow(manageSelection, 1);

            //When clicked new product is added and lists updated
            submit = new Button
            {
                Content = "Submit",
                Margin = new Thickness(5),
                Width = 130,
                FontSize = 15,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                MaxWidth = 100,
                MaxHeight = 25
            };
            submit.Click += CreateProduct;
            grid.Children.Add(submit);
            Grid.SetRow(submit, 3);
        }

        /// <summary>
        /// Switches grid depending on users choice
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            grid.Children.Remove(addGrid);
            grid.Children.Remove(removeGrid);
            grid.Children.Remove(discountGrid);
            switch (manageSelection.SelectedIndex)
            {
                case 0:
                    addGrid = itemAdd.ShowItemAdd();
                    grid.Children.Add(addGrid);
                    Grid.SetRow(addGrid, 2);
                    break;

                case 1:
                    removeGrid = itemRemove.ShowItemHandler();
                    grid.Children.Add(removeGrid);
                    Grid.SetRow(removeGrid, 2);
                    break;

                case 2:
                    discountGrid = discount.ShowDiscountGrid();
                    grid.Children.Add(discountGrid);
                    Grid.SetRow(discountGrid, 2);
                    break;
            }
        }

        /// <summary>
        /// Handles different cases for submit button depending on which grid is in view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateProduct(object sender, RoutedEventArgs e)
        {
            switch (manageSelection.SelectedIndex)
            {
                //If add product grid is up. Adds product to inventory. Checks for duplicates and if all fields have information.
                case 0:
                    bool isName = itemAdd.name.Text.Length > 0;
                    bool isDescription = itemAdd.description.Text.Length > 0;
                    bool isPrice = itemAdd.price.Text.Length > 0;
                    bool isImage = itemAdd.imageURL.Length > 0;
                    if (isName && isDescription && isPrice && isImage)
                    {
                        int indexOfLast = itemAdd.imageURL.LastIndexOf('\\');
                        File.Copy(itemAdd.imageURL, @"C:\Windows\Temp\JJSTORE\Pictures\" + itemAdd.imageURL.Substring(indexOfLast));
                        File.AppendAllText(@"C:\Windows\Temp\JJSTORE\Documents\Inventory.csv", $"{itemAdd.description.Text};{itemAdd.name.Text};{itemAdd.price.Text};{itemAdd.imageURL.Substring(indexOfLast)}\n");
                        MessageBox.Show("Item added!");
                    }
                    else if (itemAdd.currentProducts.Contains(itemAdd.name.Text.ToLower()))
                    {
                        MessageBox.Show("Article already exists!", "Error", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("Please enter all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    itemAdd.name.Text = "";
                    itemAdd.description.Text = "";
                    itemAdd.price.Text = "";
                    itemAdd.imageURLLabel.Content = "";
                    itemAdd.imageURL = "";
                    break;

                //If delete product grid is up.
                case 1:
                    MessageBox.Show("There's nothing to submit!");
                    break;

                //If discount grid is up. Checks for duplicates, adds new codes.
                case 2:
                    string key = discount.discountKeyBox.Text.ToLower();
                    decimal value = decimal.Parse(discount.discountValueBox.Text) / 100;
                    if (!discount.discountCodes.ContainsKey(key))
                    {
                        discount.discountCodes.Add(key, value);
                        discount.UpdateListBox();
                        discount.UpdateCSVFile(discount.filePath);
                    }
                    else
                    {
                        MessageBox.Show("Discount already exists!");
                    }
                    discount.discountKeyBox.Text = "";
                    discount.discountValueBox.Text = "";
                    break;

                //If start grid is up
                default:
                    MessageBox.Show("Please select an option!");
                    break;
            }
        }
    }
}