module SeatManager

type Seat = { Row: int; Col: int; IsReserved: bool }

type SeatManager() =
    // Initialize immutable seating layout as a list of seats
    let initializeLayout rows cols =
        [ for row in 0 .. rows - 1 do
            for col in 0 .. cols - 1 do
                { Row = row; Col = col; IsReserved = false } ]

    // Use an immutable Map to store layouts
    let mutable layouts = Map.empty

    member this.InitializeShowtime(showtime: string, rows: int, cols: int) =
        layouts <- layouts.Add(showtime, initializeLayout rows cols)

    member this.IsSeatAvailable(showtime: string, row: int, col: int) =
        match layouts.TryFind showtime with
        | Some seats -> seats |> List.exists (fun seat -> seat.Row = row && seat.Col = col && not seat.IsReserved)
        | None -> false

    member this.ReserveSeat(showtime: string, row: int, col: int) =
        match layouts.TryFind showtime with
        | Some seats ->
            let updatedSeats =
                seats
                |> List.map (fun seat -> if seat.Row = row && seat.Col = col then { seat with IsReserved = true } else seat)
            layouts <- layouts.Add(showtime, updatedSeats)
        | None -> ()

    member this.GetSeatLayout(showtime: string) =
        match layouts.TryFind showtime with
        | Some seats -> seats
        | None -> []

