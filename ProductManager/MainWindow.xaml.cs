using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProductManager
{
    public partial class ManagerWindow : Window
    {
        public TextBox nameBox;

        public TextBox descriptionBox;

        public TextBox priceBox;
        public Button submitButton;
        public Button imageSelectButton;
        public Label ImageURLLabel;
        public string imageURL;
        public List<string> currentProducts = new List<string>();
        public Label discountCodeLabel;
        public TextBox discountCodeBox;
        public TextBox discountValue;

        public OpenFileDialog fileDialog = new OpenFileDialog();

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

            // Scrolling
            ScrollViewer root = new ScrollViewer();
            root.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = root;

            // Main grid
            Grid grid = new Grid();
            root.Content = grid;
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            //Stack panel
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
            stackPanel.Children.Add(submitButton);
            Grid.SetRow(submitButton, 8);
            Grid.SetColumnSpan(submitButton, 2);
            Grid discountGrid = new Grid();
            stackPanel.Children.Add(discountGrid);
            discountGrid.ColumnDefinitions.Add(new ColumnDefinition());
            discountGrid.ColumnDefinitions.Add(new ColumnDefinition());
            discountGrid.ColumnDefinitions.Add(new ColumnDefinition());
            discountGrid.ColumnDefinitions.Add(new ColumnDefinition());
            discountCodeLabel = new Label
            {
                Content = "Discount code",
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            discountGrid.Children.Add(discountCodeLabel);
            Grid.SetColumn(discountCodeLabel, 0);

            discountCodeBox = new TextBox
            {
                Text = "Key",
                Width = 150,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxLength = 30
            };
            discountGrid.Children.Add(discountCodeBox);
            Grid.SetColumn(discountCodeBox, 1);
            discountValue = new TextBox
            {
                Text = "Value",
                Width = 150,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                MaxLength = 30
            };
            discountGrid.Children.Add(discountValue);
            Grid.SetColumn(discountValue, 2);
            Button submitDiscount = new Button
            {
                Content = "Submit discount",
                Margin = new Thickness(5),
                Width = 130,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyles.Italic,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            discountGrid.Children.Add(submitDiscount);
            Grid.SetColumn(submitDiscount, 3);

            currentProducts = ReadInventory(File.ReadLines(@"C:\Windows\Temp\JJSTORE\Documents\Inventory.csv").Select(a => a.Split(';')).ToList());
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

        private void CreateProduct(object sender, RoutedEventArgs e)
        {
            bool isName = nameBox.Text.Length > 0;
            bool isDescription = descriptionBox.Text.Length > 0;
            bool isPrice = priceBox.Text.Length > 0;
            bool isImage = imageURL.Length > 0;
            if (isName && isDescription && isPrice && isImage)
            {
                int indexOfLast = imageURL.LastIndexOf('\\');
                File.Copy(imageURL, @"C:\Windows\Temp\JJSTORE\Pictures\" + imageURL.Substring(indexOfLast));
                File.AppendAllText(@"C:\Windows\Temp\JJSTORE\Documents\Inventory.csv", $"{descriptionBox.Text};{nameBox.Text};{priceBox.Text};{imageURL.Substring(indexOfLast)}\n");
                MessageBox.Show("Item added!");
            }
            else if (currentProducts.Contains(nameBox.Text.ToLower()))
            {
                MessageBox.Show("Article already exists!", "Error", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("Please enter all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            nameBox.Text = "";
            descriptionBox.Text = "";
            priceBox.Text = "";
            ImageURLLabel.Content = "";
            imageURL = "";
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
}