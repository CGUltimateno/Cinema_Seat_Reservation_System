module CinemaForm
open System
open System.Drawing
open System.Windows.Forms
open SeatManager
open TicketManager
open BookingForm

type CinemaForm(seatManager: SeatManager, ticketManager: TicketManager, showtimes: string[], rows: int, cols: int) =
    let form = new Form(Text = "Cinema Seat Reservation", Size = Size(500, 500))
    let panel = new Panel(Dock = DockStyle.Fill, Location = Point(0, 100), Size = Size(500, 400))
    let showtimeDropdown = new ComboBox(Location = Point(10, 10), Width = 200)

    do
        form.Controls.Add(showtimeDropdown)
        form.Controls.Add(panel)
        showtimeDropdown.Items.AddRange(showtimes |> Array.map box)
        showtimeDropdown.SelectedIndex <- 0
        showtimes |> Array.iter (fun showtime -> seatManager.InitializeShowtime(showtime, rows, cols))

    // Calculate the starting position to center the grid
    let calculateGridStartPosition (gridWidth: int, gridHeight: int, panelWidth: int, panelHeight: int) =
        let startX = max 0 ((panelWidth - gridWidth) / 2)
        let startY = max 0 ((panelHeight - gridHeight) / 2)
        (startX, startY)

    // Redraw seats
    let redrawSeats showtime =
        panel.Controls.Clear()
        let layout = seatManager.GetSeatLayout(showtime)
        let buttonWidth, buttonHeight, spacing = 50, 40, 10
        let gridWidth = cols * (buttonWidth + spacing) - spacing
        let gridHeight = rows * (buttonHeight + spacing) - spacing
        let (startX, startY) = calculateGridStartPosition(gridWidth, gridHeight, panel.ClientSize.Width, panel.ClientSize.Height)

        layout
        |> List.iter (fun seat ->
            let button = new Button(Text = $"R{seat.Row + 1}C{seat.Col + 1}", Size = Size(buttonWidth, buttonHeight))
            button.Location <- Point(startX + seat.Col * (buttonWidth + spacing), startY + seat.Row * (buttonHeight + spacing))
            button.BackColor <- if seat.IsReserved then Color.Red else Color.Green
            button.Click.Add(fun _ ->
                if not seat.IsReserved then
                    // Open the booking form for customer details
                    let bookingForm = new BookingForm(ticketManager, showtime, seat.Row, seat.Col)
                    bookingForm.ShowBookingForm()
                    
                    // Update seat reservation after successful booking
                    if seatManager.IsSeatAvailable(showtime, seat.Row, seat.Col) then
                        seatManager.ReserveSeat(showtime, seat.Row, seat.Col)
                        button.BackColor <- Color.Red
            )
            panel.Controls.Add(button)
        )

    do showtimeDropdown.SelectedIndexChanged.Add(fun _ ->
        let selectedShowtime = showtimeDropdown.SelectedItem.ToString()
        redrawSeats selectedShowtime)

    member this.Show() =
        redrawSeats (showtimeDropdown.SelectedItem.ToString())
        Application.Run(form)
