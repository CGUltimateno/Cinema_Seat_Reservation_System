open System
open System.Drawing
open System.Windows.Forms
open System.IO

// Path: CinemaSeatReservation.fs

// Seat status: false = available, true = reserved
let mutable seatLayout = Array2D.create 5 5 false

// Function to generate a unique ticket ID
let generateTicketID () = Guid.NewGuid().ToString()

// Function to save ticket details to a file
let saveTicketToFile (ticketID: string) (customerName: string) (seat: string) (showtime: string) =
    let ticketDetails = $"Ticket ID: {ticketID}\nCustomer: {customerName}\nSeat: {seat}\nShowtime: {showtime}\n\n"
    File.AppendAllText("tickets.txt", ticketDetails)

// Function to render the seating chart GUI
let createSeatingChartForm () =
    let form = new Form(Text = "Cinema Seat Reservation", Size = Size(400, 400))

    // Create a panel to hold seat buttons
    let panel = new Panel(Dock = DockStyle.Fill)
    form.Controls.Add(panel)

    // Handle button click event
    let handleSeatClick (row: int) (col: int) (button: Button) =
        if not seatLayout.[row, col] then
            // Mark seat as reserved
            seatLayout.[row, col] <- true
            button.BackColor <- Color.Red
            button.Enabled <- false

            // Open booking form
            let bookingForm = new Form(Text = "Booking Details", Size = Size(300, 200))
            let nameLabel = new Label(Text = "Customer Name:", Location = Point(10, 10), AutoSize = true)
            let nameTextBox = new TextBox(Location = Point(120, 10), Width = 150)
            let showtimeLabel = new Label(Text = "Showtime:", Location = Point(10, 50), AutoSize = true)
            let showtimeTextBox = new TextBox(Location = Point(120, 50), Width = 150)
            let submitButton = new Button(Text = "Confirm Booking", Location = Point(100, 100), Width = 100)

            // Save ticket details and close booking form
            submitButton.Click.Add(fun _ ->
                let customerName = nameTextBox.Text
                let showtime = showtimeTextBox.Text
                let ticketID = generateTicketID ()
                let seat = $"Row {row + 1}, Col {col + 1}"
                saveTicketToFile ticketID customerName seat showtime
                ignore (MessageBox.Show($"Booking Confirmed!\nTicket ID: {ticketID}"))
                bookingForm.Close())

            bookingForm.Controls.AddRange([| nameLabel; nameTextBox; showtimeLabel; showtimeTextBox; submitButton |])
            ignore (bookingForm.ShowDialog())

    // Create seat buttons dynamically
    for row in 0 .. Array2D.length1 seatLayout - 1 do
        for col in 0 .. Array2D.length2 seatLayout - 1 do
            let button = new Button(Text = $"R{row + 1}C{col + 1}", Size = Size(60, 40), Location = Point(col * 65, row * 45))
            button.BackColor <- if seatLayout.[row, col] then Color.Red else Color.Green
            button.Enabled <- not seatLayout.[row, col]
            button.Click.Add(fun _ -> handleSeatClick row col button)
            panel.Controls.Add(button)

    form

// Entry point
[<EntryPoint>]
let main _ =
    let form = createSeatingChartForm ()
    Application.Run(form)
    0