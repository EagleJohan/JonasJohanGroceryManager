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
        public Label discountKeyLabel;

        public TextBox discountKeyBox;

        public Label discountValueLabel;

        public TextBox discountValueBox;

        public ListBox discountCodeList;

        public Dictionary<string, decimal> discountCodes = new Dictionary<string, decimal>();
        public string filePath = @"C:\Windows\Temp\JJSTORE\Documents\DiscountCodes.csv";

        public Grid ShowDiscountGrid()
        {
            ReadDiscountCodes(filePath);
            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
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
                Margin = new Thickness(5)
            };
            deleteDiscount.Click += DeleteDiscount;
            grid.Children.Add(deleteDiscount);
            Grid.SetRow(deleteDiscount, 1);
            Grid.SetColumn(deleteDiscount, 1);
            Grid.SetColumnSpan(deleteDiscount, 2);

            discountKeyLabel = new Label
            {
                Content = "Discount code",
                Margin = new Thickness(5),
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
                Content = "Discount worth in procent:",
                Margin = new Thickness(5),
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

        public void ReadDiscountCodes(string filePath)
        {
            var lines = File.ReadLines(filePath).Select(a => a.Split(';')).ToList();
            foreach (var line in lines)
            {
                discountCodes.Add(line[0], decimal.Parse(line[1]));
            }
        }

        public void UpdateListBox()
        {
            discountCodeList.Items.Clear();
            foreach (var code in discountCodes)
            {
                discountCodeList.Items.Add(code.Key);
            }
        }

        private void Integers_KeyDown(object sender, KeyEventArgs e)
        {
            var digitkeys = e.Key >= Key.D0 && e.Key <= Key.D9;
            var numbpadKeys = e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9;
            var modifiedKey = e.KeyboardDevice.Modifiers == ModifierKeys.None;
            if ((modifiedKey && (digitkeys || numbpadKeys)) || (e.Key == Key.OemComma))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

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
        public TextBox nameBox;

        public TextBox descriptionBox;

        public TextBox priceBox;

        public Button imageSelectButton;

        public Label ImageURLLabel;

        public string imageURL;

        public List<string> currentProducts = new List<string>();

        public OpenFileDialog fileDialog = new OpenFileDialog();

        public Grid ShowItemAdd()
        {
            Grid grid = new Grid();
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            grid.Children.Add(stackPanel);
            Label headerLabel = new Label
            {
                Content = "Manage products",
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            stackPanel.Children.Add(headerLabel);
            Label addProductsLabel = new Label
            {
                Content = "Add products",
                // Width = 80,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 20,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            stackPanel.Children.Add(addProductsLabel);

            Label articleLabel = new Label
            {
                Content = "Article name",
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            stackPanel.Children.Add(articleLabel);
            nameBox = new TextBox
            {
                Name = "",
                Width = 150,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxLength = 30
            };
            stackPanel.Children.Add(nameBox);

            Label descriptionLabel = new Label
            {
                Content = "Description",
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            stackPanel.Children.Add(descriptionLabel);
            descriptionBox = new TextBox
            {
                Name = "",
                Width = 150,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxLength = 200
            };
            stackPanel.Children.Add(descriptionBox);
            Label priceLabel = new Label
            {
                Content = "Price",
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            stackPanel.Children.Add(priceLabel);
            priceBox = new TextBox
            {
                Name = "",
                Width = 150,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxLength = 10
            };
            priceBox.KeyDown += Integers_KeyDown;
            stackPanel.Children.Add(priceBox);
            ImageURLLabel = new Label
            {
                Content = "",
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            stackPanel.Children.Add(ImageURLLabel);
            imageSelectButton = new Button
            {
                Content = "Add image",
                Margin = new Thickness(5),
                Width = 130,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyles.Italic,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            imageSelectButton.Click += OpenDialog;
            stackPanel.Children.Add(imageSelectButton);

            currentProducts = ReadInventory(File.ReadLines(@"C:\Windows\Temp\JJSTORE\Documents\Inventory.csv").Select(a => a.Split(';')).ToList());

            return grid;
        }

        public List<string> ReadInventory(List<string[]> file)
        {
            List<string> products = new List<string>();
            foreach (var line in file)
            {
                products.Add(line[1].ToLower());
            }

            return products;
        }

        private void Integers_KeyDown(object sender, KeyEventArgs e)
        {
            var digitkeys = e.Key >= Key.D0 && e.Key <= Key.D9;
            var numbpadKeys = e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9;
            var modifiedKey = e.KeyboardDevice.Modifiers == ModifierKeys.None;
            if ((modifiedKey && (digitkeys || numbpadKeys)) || (e.Key == Key.OemComma))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void OpenDialog(object sender, RoutedEventArgs e)
        {
            bool? isOK = true;
            if (isOK == fileDialog.ShowDialog())
            {
                imageURL = fileDialog.FileName;
                ImageURLLabel.Content = $"Image url: \n{imageURL}";
            }
        }
    }

    public class ItemRemove
    {
        public ListBox productList;
        public string filePath = @"C:\Windows\Temp\JJSTORE\Documents\Inventory.csv";
        public List<string[]> file;

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
                Margin = new Thickness(5)
            };
            delete.Click += Delete_Click;
            grid.Children.Add(delete);
            Grid.SetRow(delete, 1);

            return grid;
        }

        private void UpdateListBox()
        {
            productList.Items.Clear();
            foreach (var product in file)
            {
                productList.Items.Add(product[1]);
            }
        }

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
        public Grid grid;

        private ComboBox manageSelection;

        private ItemAdd itemAdd = new ItemAdd();
        private Grid addGrid;

        private ItemRemove itemHandler = new ItemRemove();
        private Grid handlerGrid;

        public Discount discount = new Discount();
        public Grid discountGrid;

        public Button submitButton;

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
            MinHeight = 600;
            MinWidth = 400;

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            grid = new Grid();
            root.Content = grid;
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            Label title = new Label
            {
                Content = "Product manager!",
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5)
            };
            grid.Children.Add(title);

            manageSelection = new ComboBox
            {
                Margin = new Thickness(5),
                MaxHeight = 50
            };
            manageSelection.Items.Add("Add new item");
            manageSelection.Items.Add("Manage offerings");
            manageSelection.Items.Add("Manage discounts");
            manageSelection.SelectionChanged += ManageSelection_SelectionChanged;
            grid.Children.Add(manageSelection);
            Grid.SetRow(manageSelection, 1);

            submitButton = new Button
            {
                Content = "Submit",
                Margin = new Thickness(5),
                Width = 130,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyles.Italic,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            submitButton.Click += CreateProduct;
            grid.Children.Add(submitButton);
            Grid.SetRow(submitButton, 3);
        }

        private void ManageSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            grid.Children.Remove(addGrid);
            grid.Children.Remove(handlerGrid);
            grid.Children.Remove(discountGrid);
            switch (manageSelection.SelectedIndex)
            {
                case 0:
                    addGrid = itemAdd.ShowItemAdd();
                    grid.Children.Add(addGrid);
                    Grid.SetRow(addGrid, 2);
                    break;

                case 1:
                    handlerGrid = itemHandler.ShowItemHandler();
                    grid.Children.Add(handlerGrid);
                    Grid.SetRow(handlerGrid, 2);
                    break;

                case 2:
                    discountGrid = discount.ShowDiscountGrid();
                    grid.Children.Add(discountGrid);
                    Grid.SetRow(discountGrid, 2);
                    break;
            }
        }

        private void CreateProduct(object sender, RoutedEventArgs e)
        {
            switch (manageSelection.SelectedIndex)
            {
                case 0:
                    bool isName = itemAdd.nameBox.Text.Length > 0;
                    bool isDescription = itemAdd.descriptionBox.Text.Length > 0;
                    bool isPrice = itemAdd.priceBox.Text.Length > 0;
                    bool isImage = itemAdd.imageURL.Length > 0;
                    if (isName && isDescription && isPrice && isImage)
                    {
                        int indexOfLast = itemAdd.imageURL.LastIndexOf('\\');
                        File.Copy(itemAdd.imageURL, @"C:\Windows\Temp\JJSTORE\Pictures\" + itemAdd.imageURL.Substring(indexOfLast));
                        File.AppendAllText(@"C:\Windows\Temp\JJSTORE\Documents\Inventory.csv", $"{itemAdd.descriptionBox.Text};{itemAdd.nameBox.Text};{itemAdd.priceBox.Text};{itemAdd.imageURL.Substring(indexOfLast)}\n");
                        MessageBox.Show("Item added!");
                    }
                    else if (itemAdd.currentProducts.Contains(itemAdd.nameBox.Text.ToLower()))
                    {
                        MessageBox.Show("Article already exists!", "Error", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("Please enter all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    itemAdd.nameBox.Text = "";
                    itemAdd.descriptionBox.Text = "";
                    itemAdd.priceBox.Text = "";
                    itemAdd.ImageURLLabel.Content = "";
                    itemAdd.imageURL = "";
                    break;

                case 1:
                    MessageBox.Show("There's nothing to submit!");
                    break;

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

                default:
                    MessageBox.Show("Please select an option!");
                    break;
            }
        }
    }
}