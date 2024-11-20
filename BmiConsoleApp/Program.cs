using Microsoft.VisualBasic.FileIO;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.ExceptionServices;

/// Kevin Supa
/// Apr. 24, 2024
/// <summary>
///     This program allows users to add, edit, delete, and view clients as well as manage client information like their name, height, and weight.
///     The program also calculates each clients BMI (Body Mass Index), and saves all the information in a CSV file.
/// </summary>



string mainMenuChoice;
string editClientMenuChoice;
string clientListMenuChoice;
string quit = "";
string filename = "ClientList.csv";
string loadClientConfirm;
string clientSearch;
int searchResults;
int currentClientIndex = 0;
bool validClient = false;
List<Client> clients = new List<Client>();


Console.WriteLine("/-------------------------------------------/");
Console.WriteLine("           Personal Training App"             );
Console.WriteLine("/-------------------------------------------/");

if (File.Exists(filename)) 
{ 
    LoadFile(clients, filename);
}
do
{
    MainMenu();
    if (validClient)
    {
        Console.WriteLine($"Current client: {clients[currentClientIndex].FullName}\n");
    }
    else
    {
        Console.WriteLine($"Current client: N/A\n");
    }
    mainMenuChoice = Prompt("Enter your menu selection: ").Trim().ToUpper();

    switch (mainMenuChoice)
    {
        case "V":
            if (File.Exists(filename))
            {
                do
                {
                    ClientListMenu(clients);
                    clientListMenuChoice = Prompt("Enter your menu selection: ").Trim().ToUpper();

                    switch (clientListMenuChoice)
                    {
                        case "N":
                            CreateNewClient(clients, filename);
                            Console.WriteLine("\nNew client(s) has been created.");
                            break;

                        case "L":
                            currentClientIndex = LoadClient(clients);
                            if (currentClientIndex == -1)
                            {
                                Console.WriteLine("Leaving Load Client...");
                            }
                            else if (currentClientIndex == -2)
                            {
                                Console.WriteLine("\nThat client number is not in the system.");
                            }
                            else
                            {
                                Console.WriteLine($"\nClient has been updated to {clients[currentClientIndex].FullName}.");
                                validClient = true;
                            }
                            break;

                        case "D":
                            DeleteClient(clients, filename);
                            break;

                        case "R":
                            break;

                        default:
                            Console.WriteLine("Please pick an option in the list provided.");
                            break;
                    }
                } while (clientListMenuChoice != "R");

            }
            else
            {
                Console.WriteLine("There are no clients in the system. Enter a client first.");
            }
            break;

        case "L":
            Console.WriteLine("\n-- Client Lookup --");
            clientSearch = Prompt("Enter the name of the client (Enter 0 to go back): ").ToUpper().Trim();
            if (clientSearch == "0")
            {
                Console.WriteLine("Leaving Client Lookup...");
            }
            searchResults = LookUpClient(clients, clientSearch);
            if (searchResults == 1)
            {
                loadClientConfirm = Prompt("\nWould you like to load one of these clients? (Y/N): ").ToUpper().Trim();
                if (loadClientConfirm == "Y")
                {
                    currentClientIndex = LoadClient(clients);
                    if (currentClientIndex == -1)
                    {
                        Console.WriteLine("Leaving Load Client...");
                    }
                    else if (currentClientIndex == -2)
                    {
                        Console.WriteLine("\nThat client number is not in the system.");
                    }
                    else
                    {
                        Console.WriteLine($"\nClient has been updated to {clients[currentClientIndex].FullName}.");
                        validClient = true;
                    }
                }
                else if (loadClientConfirm == "N")
                {
                    Console.WriteLine("Returning to Main Menu...");
                }
                else
                {
                    Console.WriteLine("Invalid choice. Returning to the Main Menu...");
                }
            }
            break;

        case "N":
            CreateNewClient(clients, filename);
            Console.WriteLine("\nNew client(s) has been created.");
            break;

        case "S":
            if (validClient)
            {
                BmiStatus(clients, currentClientIndex); // currentClientIndex
            }
            else
            {
                Console.WriteLine("No client has been loaded in. Select a client from the client list.");
            }
            break;

        case "E":
            if (validClient)
            {
                do
                {
                    EditClientMenu();
                    editClientMenuChoice = Prompt("Select the option you would like to edit: ").Trim().ToUpper();

                    switch (editClientMenuChoice)
                    {
                        case "F":
                            FirstNameUpdate(clients, currentClientIndex);
                            break;

                        case "L":
                            LastNameUpdate(clients, currentClientIndex);
                            break;

                        case "H":
                            HeightUpdate(clients, currentClientIndex);
                            break;

                        case "W":
                            WeightUpdate(clients, currentClientIndex);
                            break;

                        case "R":
                                UpdateFile(clients, filename);
                            break;

                        default:
                            Console.WriteLine("Please pick an option in the list provided.");
                            break;
                    }
                } while (editClientMenuChoice != "R");
            }
            else
            {
                Console.WriteLine("No client has been loaded in. Select a client from the client list.");
            }
            break;

        case "Q":
            quit = Prompt("Are you sure you would like to quit? Y/N: ").Trim().ToUpper();
            if (quit != "Y" && quit != "N")
            {
                Console.WriteLine("Invalid choice. Returning to the Main Menu...");
            }
            break;

        default:
            Console.WriteLine("Please pick an option in the list provided.");
            break;
    }
} while (quit != "Y");

Console.WriteLine("\nThank you for using this program.");









/// <summary>
/// Prompts user for input.
/// </summary>
/// <param name="prompt">Prompt for input.</param>
/// <returns>User input as a string.</returns>
static string Prompt(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine();
}

/// <summary>
/// Prompts user for an int.
/// </summary>
/// <param name="prompt">Prompt for input.</param>
/// <returns>User input as an int.</returns>
static int PromptForInt(string prompt)
{
    int response = 0;
    bool validNum = false;

    do
    {
        try
        {
            string userInput = Prompt(prompt);
            response = int.Parse(userInput);
            validNum = true;
        }
        catch (FormatException e)
        {
            Console.WriteLine("An error has occured: Please enter a valid number.\n");
        }

    } while (!validNum);

    return response;
}

/// <summary>
/// Loads data from a file and put it in a list.
/// </summary>
/// <param name="clients">The list of clients</param>
/// <param name="filename">File to load the clients from.</param>
static void LoadFile(List<Client> clients, string filename)
{
    StreamReader reader = null;

    try
    {
        reader = new StreamReader(filename);
        string clientData;

        reader.ReadLine();

        while ((clientData = reader.ReadLine()) != null)
        {
            string[] clientDataParts = clientData.Split(',');

            string firstName = clientDataParts[0].ToUpper();
            string lastName = clientDataParts[1].ToUpper();
            int weight = int.Parse(clientDataParts[2]);
            int height = int.Parse(clientDataParts[3]);

            clients.Add(new Client(firstName, lastName, weight, height));
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
    finally
    {
        if (reader != null)
        {
            // Close the reader
            reader.Close();
        }
    }
}

/// <summary>
/// Displays menu options.
/// </summary>
static void MainMenu()
{
    Console.WriteLine("\nMain Menu");
    Console.WriteLine("=========");
    Console.WriteLine("[V]iew All Clients");
    Console.WriteLine("[L]ookup Client");
    Console.WriteLine("[N]ew Client");
    Console.WriteLine("[S]how Current Client BMI Info");
    Console.WriteLine("[E]dit Current Client");
    Console.WriteLine("[Q]uit\n");
}

/// <summary>
/// Displays list of clients and client additional client menu choices.
/// </summary>
/// <param name="clients">The list of clients.</param>
static void ClientListMenu(List<Client> clients)
{
    Console.WriteLine("\nClient List");
    Console.WriteLine("==============================");
    for (int idx = 0; idx < clients.Count; idx += 1)
    {
        Console.WriteLine($"Client {idx + 1}: {clients[idx].FullName, 20}");
    }

    Console.WriteLine("\n[N]ew Client");
    Console.WriteLine("[L]oad Client");
    Console.WriteLine("[D]elete Client");
    Console.WriteLine("[R]eturn To Main Menu\n");
}

/// <summary>
/// Searches and displays the matches for a client from the client list based on the name provided by the user.
/// </summary>
/// <param name="clients">The list of clients.</param>
static int LookUpClient(List<Client> clients, string search)
{
    bool result = false;
    if (search == "0")
    {
        return 0;
    }

    Console.WriteLine("\nResults");
    Console.WriteLine("=======");
    for(int idx = 0; idx < clients.Count; idx += 1) 
    {
        if (clients[idx].FullName.Contains(search))
        {
            Console.WriteLine($"Client {idx + 1}: {clients[idx].FullName, 20}");
            result = true;
        }
    }

    if (!result)
    {
        Console.WriteLine("There are no results that match your search.");
        return 0;
    }
    else
    {
        return 1;
    }
}

/// <summary>
/// Loads a client from the client list based on the number provided by the user.
/// </summary>
/// <param name="clients">The list of clients.</param>
static int LoadClient(List<Client> clients)
{
    Console.WriteLine("\n-- Load Client --");
    int clientNum = PromptForInt("Enter the client number you would like to load (Enter 0 to go back.): ");

    for (int idx = 0; idx < clients.Count; idx += 1)
    {
        if (clientNum >= 1 && clientNum <= clients.Count)
        {
            return clientNum - 1;
        }
    }

    if (clientNum == 0)
    {
        return -1;
    }

    return -2;
}

/// <summary>
/// Deletes the client from the client list and updates the file with the new changes.
/// </summary>
/// <param name="clients">The list of clients.</param>
/// <param name="filename">File to rewrite clients to.</param>
static void DeleteClient(List<Client> clients, string filename)
{
    Console.WriteLine("\n-- Delete Client --");
    int clientNum = PromptForInt("Enter the client number you would like to delete (Enter 0 to go back): ");


    if (clientNum >= 1 && clientNum <= clients.Count)
    {
        string confirm = Prompt($"Are you sure you want to delete {clients[clientNum - 1].FullName}? (Y/N): ").ToUpper();
        if (confirm == "Y")
        {
            clients.RemoveAt(clientNum - 1);
            Console.WriteLine("\nClient has been removed from the system.");

            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(filename);
                writer.WriteLine("First Name,Last Name,Weight,Height");

                foreach (Client client in clients)
                {
                    writer.WriteLine($"{client.FirstName},{client.LastName},{client.Weight},{client.Height}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occurred: {ex.Message}");
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close(); // Close the StreamWriter
                }
            }
        }
        else if (confirm == "N")
        {
            Console.WriteLine($"{clients[clientNum - 1].FullName} was not deleted.");
        }
        else
        {
            Console.WriteLine("Invalid response. Returning to Client Menu...");
        }
    }
    else
    {
        if (clientNum == 0)
        {
            Console.WriteLine("Leaving Delete Client...");
        }
    }
}

/// <summary>
/// Displays menu options for edit client menu choice.
/// </summary>
static void EditClientMenu()
{
    Console.WriteLine("\nEdit Client Menu");
    Console.WriteLine("===============");
    Console.WriteLine("[F]irst Name");
    Console.WriteLine("[L]ast Name");
    Console.WriteLine("[H]eight");
    Console.WriteLine("[W]eight");
    Console.WriteLine("[R]eturn to Main Menu\n");
}

/// <summary>
/// Updates the first name of the client.
/// </summary>
/// <param name="clients">The list of clients</param>
/// <param name="index">The index the client is in the list.</param>
static void FirstNameUpdate(List<Client> clients, int index)
{
    Console.WriteLine($"\nCurrent Client First Name: {clients[index].FirstName}");
    try
    {
        string firstNameUpdate = Prompt("Enter the new first name (Enter 0 to go back.): ").ToUpper().Trim();
        if (firstNameUpdate == "0")
        {
            Console.WriteLine("\nReturning to Edit Client Menu...");
        }
        else if (firstNameUpdate.Any(char.IsDigit))
        {
            Console.WriteLine("An error has occured: First Name cannot include numbers.");
        }
        else
        {
            clients[index].FirstName = firstNameUpdate;
            Console.WriteLine($"\nFirst Name updated to: {clients[index].FirstName}");
        }
    } catch (Exception e) 
    { 
        Console.WriteLine($"An error has occurred: {e.Message}");
        Console.WriteLine("\nReturning to Edit Client Menu...");
    }
}

/// <summary>
/// Updates the last name of the client.
/// </summary>
/// <param name="clients">The list of clients</param>
/// <param name="index">The index the client is in the list.</param>
static void LastNameUpdate(List<Client> clients, int index)
{
    Console.WriteLine($"\nCurrent Client Last Name: {clients[index].LastName}");
    try
    {
        string lastNameUpdate = Prompt("Enter the new first name (Enter 0 to go back.): ").ToUpper().Trim();
        if (lastNameUpdate == "0")
        {
            Console.WriteLine("\nReturning to Edit Client Menu...");
        }
        else if (lastNameUpdate.Any(char.IsDigit))
        {
            Console.WriteLine("An error has occured: Last Name cannot include numbers.");
        }
        else
        {
            clients[index].LastName = lastNameUpdate;
            Console.WriteLine($"\nLast Name updated to: {clients[index].LastName}");
        }
    }
    catch (Exception e) 
    { 
        Console.WriteLine($"An error has occurred: {e.Message}");
        Console.WriteLine("\nReturning to Edit Client Menu...");
    }
}

/// <summary>
/// Updates the height of the client.
/// </summary>
/// <param name="clients">The list of clients</param>
/// <param name="index">The index the client is in the list.</param>
static void HeightUpdate(List<Client> clients, int index)
{
    Console.WriteLine($"\nCurrent Client Height: {clients[index].Height}");
    try
    {
        int heightUpdate = PromptForInt("Enter the new height (Enter 0 to go back.): ");
        if (heightUpdate == 0)
        {
            Console.WriteLine("\nReturning to Edit Client Menu...");

        }
        else
        {
            clients[index].Height = heightUpdate;
            Console.WriteLine($"\nHeight updated to: {clients[index].Height}");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"An error has occurred: {e.Message}");
        Console.WriteLine("\nReturning to Edit Client Menu...");
    }
}

/// <summary>
/// Updates the weight of the client.
/// </summary>
/// <param name="clients">The list of clients</param>
/// <param name="index">The index the client is in the list.</param>
static void WeightUpdate(List<Client> clients, int index)
{
    Console.WriteLine($"\nCurrent Client Weight: {clients[index].Weight}");
    try
    {
        int weightUpdate = PromptForInt("Enter the new weight (Enter 0 to go back.): ");
        if (weightUpdate == 0)
        {
            Console.WriteLine("\nReturning to Edit Client Menu...");

        }
        else
        {
            clients[index].Weight = weightUpdate;
            Console.WriteLine($"\nWeight updated to: {clients[index].Weight}");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"An error has occurred: {e.Message}");
        Console.WriteLine("\nReturning to Edit Client Menu...");
    }
}

/// <summary>
/// Updates the file with the modified client information.
/// </summary>
/// <param name="clients">The list of clients.</param>
/// <param name="filename">File to rewrite clients to.</param>
static void UpdateFile(List<Client> clients, string filename)
{
    StreamWriter writer = null;

    try
    {
        writer = new StreamWriter(filename);
        writer.WriteLine("First Name,Last Name,Weight,Height");

        foreach (Client client in clients)
        {
            writer.WriteLine($"{client.FirstName},{client.LastName},{client.Weight},{client.Height}");
        }

        Console.WriteLine("File has been updated with the modified client information.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error has occured: {ex.Message}");
    }
    finally
    {
        if (writer != null)
        {
            writer.Close(); // Close the StreamWriter
        }
    }
}

/// <summary>
/// Enter information for a new client which will be added to a list and saved to a file.
/// </summary>
/// <param name="clients">The list of all clients.</param>
/// <param name="filename">File to save clients to.</param>
static void CreateNewClient(List<Client> clients, string filename)
{
    string stop;

    StreamWriter writer = null;

    Console.WriteLine();
    try
    {
        if (!File.Exists(filename))
        {
            writer = new StreamWriter(filename);
            writer.WriteLine("First Name, Last Name, Weight, Height");
        }
        else
        {
            writer = new StreamWriter(filename, true);
        }

        do
        {
            string firstName = "a";
            string lastName = "a";
            int weight = 0;
            int height = 0;
            bool validFirstName = false;
            bool validLastName = false;
            bool validWeight = false;
            bool validHeight = false;
            Client client = new Client(firstName, lastName, weight, height);

            do
            {
                try
                {
                    firstName = Prompt("Enter the first name of the client: ").ToUpper().Trim();
                    if (firstName.Any(char.IsDigit))
                    {
                        Console.WriteLine("An error has occured: First Name cannot include numbers.\n");
                    }
                    else
                    {
                        client.FirstName = firstName;
                        validFirstName = true;
                    }
                }
                catch (Exception e) { Console.WriteLine($"An error has occurred: {e.Message}\n"); }
            } while (!validFirstName);

            do
            {
                try
                {
                    lastName = Prompt("Enter the last name of the client: ").ToUpper().Trim();
                    if (lastName.Any(char.IsDigit))
                    {
                        Console.WriteLine("An error has occured: Last Name cannot include numbers.\n");
                    }
                    else
                    {
                        client.LastName = lastName;
                        validLastName = true;
                    }
                }
                catch (Exception e) { Console.WriteLine($"An error has occurred: {e.Message}\n"); }
            } while (!validLastName);

            do
            {
                try
                {
                    do
                    {
                        weight = PromptForInt("Enter the weight(pounds) of the client: ");
                        if (weight == 0)
                        {
                            Console.WriteLine("An error has occured: Weight cannot be 0.\n");
                        }
                    } while (weight == 0);
  
                    client.Weight = weight;
                    validWeight = true;
                }
                catch (Exception e) { Console.WriteLine($"An error has occurred: {e.Message}\n"); }
            } while (!validWeight);

            do
            {
                try
                {
                    do
                    {
                        height = PromptForInt("Enter the height(inches) of the client: ");
                        if (height == 0)
                        {
                            Console.WriteLine("An error has occured: Height cannot be 0.\n");
                        }
                    } while (height == 0);

                    client.Height = height;
                    validHeight = true;
                }
                catch (Exception e) { Console.WriteLine($"An error has occurred: {e.Message}\n"); }
            } while (!validHeight);

            clients.Add(client);

            writer.WriteLine($"{client.FirstName}, {client.LastName}, {client.Weight}, {client.Height}");

            do
            {
                stop = Prompt("\nDo you want to enter another client? (Y/N): ").Trim().ToUpper();
                if (stop != "Y" && stop != "N")
                {
                    Console.WriteLine("Please choose Y or N.");
                }
            } while (stop != "Y" && stop != "N");

        } while (stop != "N");
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
    finally
    {
        if (writer != null)
        {
            writer.Close();
        }
    }
}

/// <summary>
/// Displays the clients full name, BMI score, and BMI status
/// </summary>
/// <param name="clients">The list of all clients.</param>
/// <param name="index">The position of the client from the client list.</param>
static void BmiStatus(List<Client> clients, int index) //ADJUST THIS FOR LIST
{
    Console.WriteLine("\n=== Client Info ===");
    Console.WriteLine($"Client Name: {clients[index].FullName, 16}");
    Console.WriteLine($"BMI Score:   {clients[index].BmiScore, 16:0.00}");
    Console.WriteLine($"BMI Status:  {clients[index].BmiStatus, 16}");

    Console.Write("\nPress Enter to Continue...");
    Console.ReadLine();
}