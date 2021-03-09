using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace ProductManager
{
    public partial class ManagerWindow : Window
    {
        public TextBox nameBox;

        public TextBox descriptionBox;

        public TextBox priceBox;

        public Button imageSelectButton;
        public Label ImageURLLabel;
        public string imageURL;

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

            Label nameLabel = new Label
            {
                Content = "Article name",
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            stackPanel.Children.Add(nameLabel);
            nameBox = new TextBox
            {
                Name = "",
                Width = 150,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
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
                HorizontalAlignment = HorizontalAlignment.Left
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
                HorizontalAlignment = HorizontalAlignment.Left
            };
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
            Button submitButton = new Button
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
        }

        private void CreateProduct(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
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