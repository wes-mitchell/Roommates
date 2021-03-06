using System;
using System.Collections.Generic;
using Roommates.Repositories;
using Roommates.Models;
using System.Linq;

namespace Roommates
{
    class Program
    {
        //  This is the address of the database.
        //  We define it here as a constant since it will never change.
        private const string CONNECTION_STRING = @"server=localhost\SQLExpress;database=Roommates;integrated security=true;TrustServerCertificate=true;";

        static void Main(string[] args)
        {
            RoomRepository roomRepo = new RoomRepository(CONNECTION_STRING);
            ChoreRepository choreRepo = new ChoreRepository(CONNECTION_STRING);
            RoommateRepository roomieRepo = new RoommateRepository(CONNECTION_STRING);

            bool runProgram = true;
            while (runProgram)
            {
                string selection = GetMenuSelection();

                switch (selection)
                {
                    case ("Show all rooms"):
                        List<Room> rooms = roomRepo.GetAll();
                        foreach (Room r in rooms)
                        {
                            Console.WriteLine($"{r.Name} has an Id of {r.Id} and a max occupancy of {r.MaxOccupancy}");
                        }
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Show all chores"):
                        List<Chore> chores = choreRepo.GetAll();
                        foreach (Chore c in chores)
                        {
                            Console.WriteLine($"{c.Name}");
                        }
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Search for room"):
                        Console.Write("Room Id: ");
                        int id = int.Parse(Console.ReadLine());

                        Room room = roomRepo.GetById(id);

                        Console.WriteLine($"{room.Id} - {room.Name} Max Occupancy({room.MaxOccupancy})");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Search for chore"):
                        Console.Write("Chore Id: ");
                        int choreId = int.Parse(Console.ReadLine());

                        Chore chore = choreRepo.GetById(choreId);

                        Console.WriteLine($"{chore.Id} - {chore.Name}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("See unassigned chores"):
                        List<Chore> unChores = choreRepo.GetUnassignedChores();
                        foreach(Chore c in unChores)
                        {
                            Console.WriteLine($"{c.Name} still needs assigned to a room mate.");
                        }
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Search for roommate"):
                        Console.Write("Roommate Id: ");
                        int roomieId = int.Parse(Console.ReadLine());

                        Roommate roomie = roomieRepo.GetById(roomieId);

                        Console.WriteLine($"Roommate Id: {roomie.Id} Name: {roomie.FirstName} Room: {roomie.Room.Name} Rent Portion: {roomie.RentPortion}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Add a room"):
                        Console.Write("Room name: ");
                        string name = Console.ReadLine();

                        Console.Write("Max occupancy: ");
                        int max = int.Parse(Console.ReadLine());

                        Room roomToAdd = new Room()
                        {
                            Name = name,
                            MaxOccupancy = max
                        };

                        roomRepo.Insert(roomToAdd);

                        Console.WriteLine($"{roomToAdd.Name} has been added and assigned an Id of {roomToAdd.Id}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Add a chore"):
                        Console.Write("Chore name: ");
                        string choreName = Console.ReadLine();

                        Chore choreToAdd = new Chore()
                        {
                            Name = choreName
                        };

                        choreRepo.Insert(choreToAdd);

                        Console.WriteLine($"{choreToAdd.Name} has been added and assigned an Id of {choreToAdd.Id}");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Update a chore"):
                        List<Chore> choreList = choreRepo.GetAll();
                        foreach (Chore c in choreList)
                        {
                            Console.WriteLine($"{c.Id} - {c.Name}");
                        }

                        Console.Write("Select a chore you'd like to update: ");
                        int selectedChore = int.Parse(Console.ReadLine());
                        Chore selChore = choreList.FirstOrDefault(c => c.Id == selectedChore);

                        Console.Write("New Chore Name: ");
                        selChore.Name = Console.ReadLine();
                        selChore.Id = selectedChore;

                        choreRepo.Update(selChore);
                        Console.WriteLine($"Success, you renamed the chore to {selChore.Name}.");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Delete a chore"):
                        List<Chore> deleteChoreList = choreRepo.GetAll();
                        foreach (Chore c in deleteChoreList)
                        {
                            Console.WriteLine($"{c.Id} - {c.Name}");
                        }

                        Console.WriteLine("Which id would you like to delete? ");
                        int userSelection = int.Parse(Console.ReadLine());

                        choreRepo.Delete(userSelection);

                        Console.WriteLine($"You've successfully deleted the chore!");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Assign roommate a chore"):
                        List<Roommate> allRoommates = roomieRepo.GetAll();

                        List<Chore> allChores = choreRepo.GetAll();

                        int roomieNum = 1;
                        int choreNum = 1;

                        foreach (Roommate r in allRoommates)
                        {
                            Console.WriteLine($"{roomieNum}) {r.FirstName} {r.LastName}");
                            roomieNum++;
                        }

                        int roomieSelection;
                        Console.Write("Select a roomie by number: ");
                        roomieSelection = int.Parse(Console.ReadLine());

                        foreach(Chore c in allChores)
                        {
                            Console.WriteLine($"{choreNum}) {c.Name}");
                            choreNum++;
                        }

                        int choreSelection;
                        Console.Write("Select a chore by number: ");
                        choreSelection = int.Parse(Console.ReadLine());

                        choreRepo.AssignChore(roomieSelection, choreSelection);


                        Console.WriteLine($"{roomieSelection}, {choreSelection}");
                        Console.WriteLine($"Operation successful");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Update a room"):
                        List<Room> roomOptions = roomRepo.GetAll();
                        foreach (Room r in roomOptions)
                        {
                            Console.WriteLine($"{r.Id} - {r.Name}");
                        }

                        Console.Write("Which room would you like to update? ");
                        int selectedRoomId = int.Parse(Console.ReadLine());
                        Room selectedRoom = roomOptions.FirstOrDefault(r => r.Id == selectedRoomId);

                        Console.Write("New Name: ");
                        selectedRoom.Name = Console.ReadLine();

                        Console.Write("New Max Occupancy: ");
                        selectedRoom.MaxOccupancy = int.Parse(Console.ReadLine());

                        roomRepo.Update(selectedRoom);

                        Console.WriteLine("Room has been successfully updated");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Delete a room"):
                        List<Room> allRooms = roomRepo.GetAll();
                        foreach(Room r in allRooms)
                        {
                            Console.WriteLine($"{r.Id} - {r.Name}");
                        }

                        Console.Write("Which room would you like to delete? ");
                        int roomId = int.Parse(Console.ReadLine());

                        roomRepo.Delete(roomId);

                        Console.WriteLine("That room is toast, bye bye.");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                    case ("Exit"):
                        runProgram = false;
                        break;
                    case ("See roommate chore count"):
                        List<ChoreCount> choreCount = choreRepo.GetChoreCount();
                        foreach (ChoreCount c in choreCount)
                        {
                            Console.WriteLine($"{c.Name}: {c.ChoreAmount}");
                        }
                        Console.WriteLine("Make sure you make that even boy.");
                        Console.Write("Press any key to continue");
                        Console.ReadKey();
                        break;
                }
            }

        }

        static string GetMenuSelection()
        {
            Console.Clear();

            List<string> options = new List<string>()
            {
                "Show all rooms",
                "Show all chores",
                "Search for chore",
                "See unassigned chores",
                "Search for room",
                "Search for roommate",
                "Add a chore",
                "Update a chore",
                "Delete a chore",
                "Add a room",
                "Delete a room",
                "Update a room",
                "Assign roommate a chore",
                "See roommate chore count",
                "Exit"
            };

            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }

            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.Write("Select an option > ");

                    string input = Console.ReadLine();
                    int index = int.Parse(input) - 1;
                    return options[index];
                }
                catch (Exception)
                {

                    continue;
                }
            }
        }
    }
}
