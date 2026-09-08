using Lab1_OOP_Bradul.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;

namespace Lab1_OOP_Bradul
{
    public sealed partial class MainWindow : Window
    {
        private readonly List<Flight> flights = new List<Flight>();

        private Flight? selectedFlight;

        private int maxFlights = 0;


        public MainWindow()
        {
            this.InitializeComponent();
        }


        // ==========================================
        // MAXIMUM NUMBER OF OBJECTS
        // ==========================================

        private async void RootGrid_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            if (maxFlights > 0)
                return;


            NumberBox limitBox = new NumberBox
            {
                Header = "Maximum number of flights",
                Minimum = 1,
                Value = 10,

                SpinButtonPlacementMode =
                    NumberBoxSpinButtonPlacementMode.Compact
            };


            TextBlock errorText = CreateErrorText();


            StackPanel panel = new StackPanel
            {
                Spacing = 10
            };

            panel.Children.Add(
                new TextBlock
                {
                    Text =
                        "Enter how many Flight objects the program can store.",
                    TextWrapping = TextWrapping.Wrap
                });

            panel.Children.Add(limitBox);
            panel.Children.Add(errorText);


            ContentDialog dialog = new ContentDialog
            {
                Title = "Set storage limit",
                PrimaryButtonText = "Continue",
                DefaultButton = ContentDialogButton.Primary,
                Content = panel,
                XamlRoot = RootGrid.XamlRoot
            };


            dialog.PrimaryButtonClick +=
                (dialogSender, args) =>
                {
                    errorText.Text = "";


                    if (double.IsNaN(limitBox.Value) ||
                        limitBox.Value <= 0 ||
                        limitBox.Value % 1 != 0)
                    {
                        errorText.Text =
                            "Enter a positive whole number.";

                        args.Cancel = true;
                        return;
                    }


                    maxFlights =
                        (int)limitBox.Value;
                };


            ContentDialogResult result =
                await dialog.ShowAsync();


            if (result == ContentDialogResult.Primary &&
                maxFlights > 0)
            {
                AddFlightButton.IsEnabled = true;

                FlightLimitText.Text =
                    $"Limit: {maxFlights}";

                RefreshFlightList();
            }
        }


        // ==========================================
        // ADD FLIGHT
        // ==========================================

        private async void AddFlightButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (maxFlights <= 0)
                return;


            if (flights.Count >= maxFlights)
            {
                await ShowMessage(
                    "Limit reached",
                    $"The program can store a maximum of {maxFlights} flights.");

                return;
            }


            TextBox flightNumberBox = new TextBox
            {
                Header = "Flight number",
                PlaceholderText = "For example: LH148"
            };


            TextBox destinationBox = new TextBox
            {
                Header = "Destination",
                PlaceholderText = "For example: Berlin"
            };


            CalendarDatePicker datePicker =
                new CalendarDatePicker
                {
                    Header = "Departure date",
                    PlaceholderText = "Select date"
                };


            TimePicker timePicker =
                new TimePicker
                {
                    Header = "Departure time"
                };


            NumberBox passengersBox =
                new NumberBox
                {
                    Header = "Passengers",
                    Minimum = 0,
                    Maximum = 500,
                    Value = 0,

                    SpinButtonPlacementMode =
                        NumberBoxSpinButtonPlacementMode.Compact
                };


            NumberBox priceBox =
                new NumberBox
                {
                    Header = "Ticket price (ˆ)",
                    Minimum = 0.01,
                    Maximum = 10000,
                    Value = 100,

                    SpinButtonPlacementMode =
                        NumberBoxSpinButtonPlacementMode.Compact
                };


            ComboBox statusBox = new ComboBox
            {
                Header = "Status",
                HorizontalAlignment =
                    HorizontalAlignment.Stretch
            };


            foreach (
                FlightStatus status
                in Enum.GetValues(typeof(FlightStatus)))
            {
                statusBox.Items.Add(
                    status.ToString());
            }


            statusBox.SelectedIndex = 0;


            CheckBox internationalBox =
                new CheckBox
                {
                    Content =
                        "International flight"
                };


            TextBlock errorText =
                CreateErrorText();


            StackPanel panel = new StackPanel
            {
                Spacing = 11
            };


            panel.Children.Add(flightNumberBox);
            panel.Children.Add(destinationBox);
            panel.Children.Add(datePicker);
            panel.Children.Add(timePicker);
            panel.Children.Add(passengersBox);
            panel.Children.Add(priceBox);
            panel.Children.Add(statusBox);
            panel.Children.Add(internationalBox);
            panel.Children.Add(errorText);


            ContentDialog dialog =
                new ContentDialog
                {
                    Title = "Add new flight",

                    PrimaryButtonText =
                        "Add flight",

                    CloseButtonText =
                        "Cancel",

                    DefaultButton =
                        ContentDialogButton.Primary,

                    Content = panel,

                    XamlRoot =
                        RootGrid.XamlRoot
                };


            dialog.PrimaryButtonClick +=
                (dialogSender, args) =>
                {
                    errorText.Text = "";


                    // Flight number

                    string flightNumber =
                        flightNumberBox.Text.Trim();


                    if (string.IsNullOrWhiteSpace(
                            flightNumber) ||
                        flightNumber.Length < 2 ||
                        flightNumber.Length > 7)
                    {
                        errorText.Text =
                            "Flight number must contain 2 to 7 characters.";

                        args.Cancel = true;
                        return;
                    }


                    // Destination

                    string destination =
                        destinationBox.Text.Trim();


                    if (string.IsNullOrWhiteSpace(
                            destination))
                    {
                        errorText.Text =
                            "Destination cannot be empty.";

                        args.Cancel = true;
                        return;
                    }


                    // Date

                    if (datePicker.Date == null)
                    {
                        errorText.Text =
                            "Select the departure date.";

                        args.Cancel = true;
                        return;
                    }


                    DateTime date =
                        datePicker.Date.Value.DateTime;


                    TimeSpan time =
                        timePicker.Time;


                    DateTime departure =
                        date.Date + time;


                    if (departure <= DateTime.Now)
                    {
                        errorText.Text =
                            "Departure time must be in the future.";

                        args.Cancel = true;
                        return;
                    }


                    // Passengers

                    if (double.IsNaN(
                            passengersBox.Value) ||
                        passengersBox.Value < 0 ||
                        passengersBox.Value > 500 ||
                        passengersBox.Value % 1 != 0)
                    {
                        errorText.Text =
                            "Passengers must be a whole number from 0 to 500.";

                        args.Cancel = true;
                        return;
                    }


                    // Price

                    if (double.IsNaN(
                            priceBox.Value) ||
                        priceBox.Value <= 0 ||
                        priceBox.Value > 10000)
                    {
                        errorText.Text =
                            "Ticket price must be greater than 0 and not more than 10000.";

                        args.Cancel = true;
                        return;
                    }


                    // Status

                    if (statusBox.SelectedIndex < 0)
                    {
                        errorText.Text =
                            "Select a flight status.";

                        args.Cancel = true;
                        return;
                    }


                    FlightStatus status =
                        (FlightStatus)
                        statusBox.SelectedIndex;


                    Flight newFlight =
                        new Flight(
                            flightNumber.ToUpper(),
                            destination,
                            departure,
                            (int)passengersBox.Value,
                            priceBox.Value,
                            status,
                            internationalBox.IsChecked == true
                        );


                    flights.Add(newFlight);


                    SearchTextBox.Text = "";


                    RefreshFlightList();
                };


            await dialog.ShowAsync();
        }


        // ==========================================
        // DISPLAY FLIGHTS
        // ==========================================

        private void DisplayFlights(
            List<Flight> flightsToShow)
        {
            FlightsListView.Items.Clear();


            ListViewItem? itemToSelect =
                null;


            foreach (Flight flight
                     in flightsToShow)
            {
                int number =
                    flights.IndexOf(flight) + 1;


                Grid row = new Grid
                {
                    Padding =
                        new Thickness(
                            8, 13, 8, 13)
                };


                row.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width =
                            new GridLength(35)
                    });

                row.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width =
                            new GridLength(70)
                    });

                row.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width =
                            new GridLength(120)
                    });

                row.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width =
                            new GridLength(135)
                    });

                row.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width =
                            new GridLength(85)
                    });

                row.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width =
                            new GridLength(80)
                    });

                row.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width =
                            new GridLength(100)
                    });

                row.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width =
                            new GridLength(90)
                    });


                row.Children.Add(
                    CreateCell(
                        number.ToString(),
                        0));


                row.Children.Add(
                    CreateCell(
                        flight.FlightNumber,
                        1,
                        true));


                row.Children.Add(
                    CreateCell(
                        flight.Destination,
                        2));


                row.Children.Add(
                    CreateCell(
                        flight.DepartureTime
                            .ToString(
                                "dd.MM.yyyy HH:mm"),
                        3));


                row.Children.Add(
                    CreateCell(
                        flight.PassengerCount
                            .ToString(),
                        4));


                row.Children.Add(
                    CreateCell(
                        $"ˆ{flight.TicketPrice:F2}",
                        5));


                row.Children.Add(
                    CreateCell(
                        flight.IsInternational
                            ? "International"
                            : "Domestic",
                        6));


                row.Children.Add(
                    CreateCell(
                        flight.Status.ToString(),
                        7));


                ListViewItem item =
                    new ListViewItem
                    {
                        Content = row,

                        Tag = flight,

                        HorizontalContentAlignment =
                            HorizontalAlignment.Stretch
                    };


                if (flight == selectedFlight)
                {
                    itemToSelect = item;
                }


                FlightsListView.Items.Add(
                    item);
            }


            if (flightsToShow.Count == 0)
            {
                EmptyFlightsPanel.Visibility =
                    Visibility.Visible;


                if (flights.Count == 0)
                {
                    EmptyFlightsTitleText.Text =
                        "No flights yet";

                    EmptyFlightsSubtitleText.Text =
                        "Add your first flight to get started";
                }
                else
                {
                    EmptyFlightsTitleText.Text =
                        "No matching flights";

                    EmptyFlightsSubtitleText.Text =
                        "Try another search value";
                }
            }
            else
            {
                EmptyFlightsPanel.Visibility =
                    Visibility.Collapsed;
            }


            if (itemToSelect != null)
            {
                FlightsListView.SelectedItem =
                    itemToSelect;
            }
        }


        // ==========================================
        // REFRESH ALL FLIGHTS
        // ==========================================

        private void RefreshFlightList()
        {
            DisplayFlights(flights);


            FlightCountText.Text =
                maxFlights > 0
                    ? $"{flights.Count} / {maxFlights} flights"
                    : $"{flights.Count} flights";


            DeleteFlightButton.IsEnabled =
                flights.Count > 0;
        }


        // ==========================================
        // SEARCH
        // ==========================================

        private async void SearchButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string searchValue =
                SearchTextBox.Text.Trim();


            if (string.IsNullOrWhiteSpace(
                    searchValue))
            {
                ClearSelectedFlight();

                RefreshFlightList();

                return;
            }


            List<Flight> foundFlights =
                new List<Flight>();


            foreach (Flight flight in flights)
            {
                bool matches = false;


                // Search by flight number

                if (SearchFieldComboBox
                        .SelectedIndex == 0)
                {
                    matches =
                        flight.FlightNumber
                            .IndexOf(
                                searchValue,
                                StringComparison
                                    .OrdinalIgnoreCase)
                        >= 0;
                }


                // Search by destination

                else if (
                    SearchFieldComboBox
                        .SelectedIndex == 1)
                {
                    matches =
                        flight.Destination
                            .IndexOf(
                                searchValue,
                                StringComparison
                                    .OrdinalIgnoreCase)
                        >= 0;
                }


                if (matches)
                {
                    foundFlights.Add(flight);
                }
            }


            ClearSelectedFlight();


            DisplayFlights(foundFlights);


            if (foundFlights.Count == 0)
            {
                FlightCountText.Text =
                    "0 found";


                await ShowMessage(
                    "No flights found",
                    "No flights match the specified search value.");

                return;
            }


            FlightCountText.Text =
                foundFlights.Count == 1
                    ? "1 found"
                    : $"{foundFlights.Count} found";
        }


        // ==========================================
        // SELECT FLIGHT
        // ==========================================

        private void FlightsListView_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (FlightsListView.SelectedItem
                    is ListViewItem item &&
                item.Tag is Flight flight)
            {
                selectedFlight = flight;


                ShowSelectedFlight();


                AddPassengerButton.IsEnabled = true;
                DelayFlightButton.IsEnabled = true;
                StartBoardingButton.IsEnabled = true;
                CancelFlightButton.IsEnabled = true;
            }
        }


        // ==========================================
        // SHOW SELECTED FLIGHT
        // ==========================================

        private void ShowSelectedFlight()
        {
            if (selectedFlight == null)
                return;


            SelectedFlightNumberText.Text =
                selectedFlight.FlightNumber;


            SelectedDestinationText.Text =
                selectedFlight.Destination;


            SelectedDepartureText.Text =
                selectedFlight.DepartureTime
                    .ToString(
                        "dd.MM.yyyy HH:mm");


            SelectedPassengersText.Text =
                selectedFlight.PassengerCount
                    .ToString();


            SelectedPriceText.Text =
                $"ˆ{selectedFlight.TicketPrice:F2}";


            SelectedTypeText.Text =
                selectedFlight.IsInternational
                    ? "International"
                    : "Domestic";


            SelectedStatusText.Text =
                selectedFlight.Status
                    .ToString();
        }


        // ==========================================
        // CLEAR SELECTED FLIGHT
        // ==========================================

        private void ClearSelectedFlight()
        {
            selectedFlight = null;


            FlightsListView.SelectedItem =
                null;


            SelectedFlightNumberText.Text = "-";
            SelectedDestinationText.Text = "-";
            SelectedDepartureText.Text = "-";
            SelectedPassengersText.Text = "-";
            SelectedPriceText.Text = "-";
            SelectedTypeText.Text = "-";

            SelectedStatusText.Text =
                "Not selected";


            AddPassengerButton.IsEnabled = false;
            DelayFlightButton.IsEnabled = false;
            StartBoardingButton.IsEnabled = false;
            CancelFlightButton.IsEnabled = false;
        }


        // ==========================================
        // METHOD 1 - ADD PASSENGER
        // ==========================================

        private async void AddPassengerButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (selectedFlight == null)
                return;


            bool added =
                selectedFlight.AddPassenger();


            if (!added)
            {
                await ShowMessage(
                    "Cannot add passenger",
                    "Maximum number of passengers is 500.");

                return;
            }


            RefreshFlightList();

            ShowSelectedFlight();
        }


        // ==========================================
        // METHOD 2 - DELAY FLIGHT
        // ==========================================

        private async void DelayFlightButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (selectedFlight == null)
                return;


            NumberBox minutesBox =
                new NumberBox
                {
                    Header =
                        "Delay time in minutes",

                    Minimum = 1,

                    Maximum = 1440,

                    Value = 15,

                    SpinButtonPlacementMode =
                        NumberBoxSpinButtonPlacementMode
                            .Compact
                };


            ContentDialog dialog =
                new ContentDialog
                {
                    Title = "Delay flight",

                    PrimaryButtonText =
                        "Delay",

                    CloseButtonText =
                        "Cancel",

                    Content = minutesBox,

                    XamlRoot =
                        RootGrid.XamlRoot
                };


            ContentDialogResult result =
                await dialog.ShowAsync();


            if (result ==
                ContentDialogResult.Primary)
            {
                selectedFlight.DelayFlight(
                    (int)minutesBox.Value);


                RefreshFlightList();

                ShowSelectedFlight();
            }
        }


        // ==========================================
        // METHOD 3 - START BOARDING
        // ==========================================

        private void StartBoardingButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (selectedFlight == null)
                return;


            selectedFlight.StartBoarding();


            RefreshFlightList();

            ShowSelectedFlight();
        }


        // ==========================================
        // METHOD 4 - CANCEL FLIGHT
        // ==========================================

        private void CancelFlightButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (selectedFlight == null)
                return;


            selectedFlight.CancelFlight();


            RefreshFlightList();

            ShowSelectedFlight();
        }


        // ==========================================
        // DELETE
        // ==========================================

        private async void DeleteFlightButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (flights.Count == 0)
                return;


            ComboBox deleteTypeBox =
                new ComboBox
                {
                    Header = "Delete by",

                    HorizontalAlignment =
                        HorizontalAlignment.Stretch
                };


            deleteTypeBox.Items.Add(
                "Table number");

            deleteTypeBox.Items.Add(
                "Flight number");

            deleteTypeBox.Items.Add(
                "Destination");


            deleteTypeBox.SelectedIndex = 0;


            TextBox valueBox =
                new TextBox
                {
                    Header = "Value",

                    PlaceholderText =
                        "Enter value"
                };


            if (selectedFlight != null)
            {
                valueBox.Text =
                    (flights.IndexOf(
                        selectedFlight) + 1)
                    .ToString();
            }


            TextBlock informationText =
                new TextBlock
                {
                    Text =
                        "Deleting by Flight number or Destination removes all matching flights.",

                    TextWrapping =
                        TextWrapping.Wrap,

                    Foreground =
                        new SolidColorBrush(
                            Windows.UI.Color.FromArgb(
                                255,
                                120,
                                135,
                                155))
                };


            TextBlock errorText =
                CreateErrorText();


            StackPanel panel =
                new StackPanel
                {
                    Spacing = 11
                };


            panel.Children.Add(
                deleteTypeBox);

            panel.Children.Add(
                valueBox);

            panel.Children.Add(
                informationText);

            panel.Children.Add(
                errorText);


            int deletedCount = 0;


            ContentDialog dialog =
                new ContentDialog
                {
                    Title = "Delete flights",

                    PrimaryButtonText =
                        "Delete",

                    CloseButtonText =
                        "Cancel",

                    Content = panel,

                    XamlRoot =
                        RootGrid.XamlRoot
                };


            dialog.PrimaryButtonClick +=
                (dialogSender, args) =>
                {
                    errorText.Text = "";

                    deletedCount = 0;


                    string value =
                        valueBox.Text.Trim();


                    if (string.IsNullOrWhiteSpace(
                            value))
                    {
                        errorText.Text =
                            "Enter a value.";

                        args.Cancel = true;
                        return;
                    }


                    // DELETE BY TABLE NUMBER

                    if (deleteTypeBox
                            .SelectedIndex == 0)
                    {
                        if (!int.TryParse(
                                value,
                                out int number) ||
                            number < 1 ||
                            number > flights.Count)
                        {
                            errorText.Text =
                                $"Enter a number from 1 to {flights.Count}.";

                            args.Cancel = true;
                            return;
                        }


                        flights.RemoveAt(
                            number - 1);


                        deletedCount = 1;
                    }


                    // DELETE BY FLIGHT NUMBER

                    else if (
                        deleteTypeBox
                            .SelectedIndex == 1)
                    {
                        for (
                            int i =
                                flights.Count - 1;
                            i >= 0;
                            i--)
                        {
                            if (string.Equals(
                                    flights[i]
                                        .FlightNumber,
                                    value,
                                    StringComparison
                                        .OrdinalIgnoreCase))
                            {
                                flights.RemoveAt(i);

                                deletedCount++;
                            }
                        }


                        if (deletedCount == 0)
                        {
                            errorText.Text =
                                "No flights with this flight number were found.";

                            args.Cancel = true;
                            return;
                        }
                    }


                    // DELETE BY DESTINATION

                    else if (
                        deleteTypeBox
                            .SelectedIndex == 2)
                    {
                        for (
                            int i =
                                flights.Count - 1;
                            i >= 0;
                            i--)
                        {
                            if (string.Equals(
                                    flights[i]
                                        .Destination,
                                    value,
                                    StringComparison
                                        .OrdinalIgnoreCase))
                            {
                                flights.RemoveAt(i);

                                deletedCount++;
                            }
                        }


                        if (deletedCount == 0)
                        {
                            errorText.Text =
                                "No flights with this destination were found.";

                            args.Cancel = true;
                            return;
                        }
                    }
                };


            ContentDialogResult result =
                await dialog.ShowAsync();


            if (result ==
                    ContentDialogResult.Primary &&
                deletedCount > 0)
            {
                ClearSelectedFlight();


                SearchTextBox.Text = "";


                RefreshFlightList();


                await ShowMessage(
                    "Deletion completed",

                    deletedCount == 1
                        ? "1 flight was deleted."
                        : $"{deletedCount} flights were deleted.");
            }
        }


        // ==========================================
        // EXIT
        // ==========================================

        private void ExitButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            this.Close();
        }


        // ==========================================
        // HELPER - TABLE CELL
        // ==========================================

        private TextBlock CreateCell(
            string text,
            int column,
            bool bold = false)
        {
            TextBlock block =
                new TextBlock
                {
                    Text = text,

                    VerticalAlignment =
                        VerticalAlignment.Center,

                    Foreground =
                        new SolidColorBrush(
                            Windows.UI.Color.FromArgb(
                                255,
                                71,
                                85,
                                105)),

                    TextTrimming =
                        TextTrimming.CharacterEllipsis
                };


            if (bold)
            {
                block.FontWeight =
                    Microsoft.UI.Text
                        .FontWeights.SemiBold;
            }


            Grid.SetColumn(
                block,
                column);


            return block;
        }


        // ==========================================
        // HELPER - ERROR TEXT
        // ==========================================

        private TextBlock CreateErrorText()
        {
            return new TextBlock
            {
                Foreground =
                    new SolidColorBrush(
                        Windows.UI.Color.FromArgb(
                            255,
                            190,
                            70,
                            75)),

                TextWrapping =
                    TextWrapping.Wrap
            };
        }


        // ==========================================
        // HELPER - MESSAGE
        // ==========================================

        private async System.Threading.Tasks.Task
            ShowMessage(
                string title,
                string message)
        {
            ContentDialog dialog =
                new ContentDialog
                {
                    Title = title,

                    Content = message,

                    CloseButtonText =
                        "OK",

                    XamlRoot =
                        RootGrid.XamlRoot
                };


            await dialog.ShowAsync();
        }
    }
}