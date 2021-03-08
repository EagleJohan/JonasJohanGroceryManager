using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProductManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            // Window options
            Title = "Manage Products";
            Width = 300;
            Height = 450;
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
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70, GridUnitType.Star) });
            grid.ShowGridLines = true;
            Label headerLabel = CreateLabel("Manage Products", grid, 0, 0, 20);
            Grid.SetColumnSpan(headerLabel, 2);

            Label addProductsLabel = CreateLabel("Add New Product", grid, 2, 0, 15);
            Grid.SetColumnSpan(addProductsLabel, 2);

            Label idLabel = CreateLabel("ID: ", grid, 3, 0, 12);

            Label nameLabel = CreateLabel("Name: ", grid, 4, 0, 12);

            Label descriptionLabel = CreateLabel("Description: ", grid, 5, 0, 12);

            Label priceLabel = CreateLabel("Price: ", grid, 6, 0, 12);

            Label imageLabel = CreateLabel("Image URL: ", grid, 7, 0, 12);

            TextBox idBox = CreateTextbox(grid, 3, 2);

            TextBox nameBox = CreateTextbox(grid, 4, 2);

            TextBox descriptionBox = CreateTextbox(grid, 5, 2);

            TextBox priceBox = CreateTextbox(grid, 6, 2);

            TextBox imageBox = CreateTextbox(grid, 7, 2);

            Button submitButton = new Button
            {
                Content = "Submit",
                Width = 130,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                FontStyle = FontStyles.Italic,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            grid.Children.Add(submitButton);
            Grid.SetRow(submitButton, 8);
            Grid.SetColumnSpan(submitButton, 2);

            ComboBox productList = new ComboBox
            {
                Name = "Productlist"
            };

        }
        private Label CreateLabel(string content, Grid grid, int row, int column, int fontsize)
        {
            Label label = new Label
            {
                Content = content,
                // Width = 80,
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

        private TextBox CreateTextbox(Grid grid, int row, int column)
        {
            TextBox textbox = new TextBox
            {
                Name = "",
                Width = 150,
                Margin = new Thickness(5),
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            grid.Children.Add(textbox);
            Grid.SetRow(textbox, row);
            Grid.SetColumn(textbox, column);
            return textbox;
        }
    }
    }
}
