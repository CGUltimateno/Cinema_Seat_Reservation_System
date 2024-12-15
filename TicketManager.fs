module TicketManager
open System

type Ticket = { TicketID: string; CustomerName: string; Seat: string; Showtime: string }

type TicketManager() =
    member this.GenerateTicketID() = Guid.NewGuid().ToString()
    member this.SaveTicket(ticket: Ticket) =
        let ticketDetails = $"Ticket ID: {ticket.TicketID}\nCustomer: {ticket.CustomerName}\nSeat: {ticket.Seat}\nShowtime: {ticket.Showtime}\n\n"
        System.IO.File.AppendAllText("bookings.txt", ticketDetails)
