﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            if (crudOperation == "create")
            {
                db.Employees.InsertOnSubmit(employee);
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    db.SubmitChanges();
                }
            }
            else if (crudOperation == "delete")
            {

                Employee employeeFromDb = db.Employees.Where(a => a.EmployeeId == employee.EmployeeId).FirstOrDefault();
                db.Employees.DeleteOnSubmit(employeeFromDb);

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Provide for exceptions.
                }
            }
            else if (crudOperation == "read")
            {
                Employee employeeFromDb = db.Employees.Where(a => a == employee).FirstOrDefault();
                Console.WriteLine(employeeFromDb.FirstName + "\n" +
                    employeeFromDb.LastName + "\n" +
                    employeeFromDb.UserName + "\n" +
                    employeeFromDb.Password + "\n" +
                    employeeFromDb.EmployeeNumber + "\n" +
                    employeeFromDb.Email);
            }
            else if (crudOperation == "update")
            {
                Employee employeeFromDb = db.Employees.Where(a => a.EmployeeId == employee.EmployeeId).FirstOrDefault();
                employeeFromDb.FirstName = employee.FirstName;
                employeeFromDb.LastName = employee.LastName;
                employeeFromDb.UserName = employee.UserName;
                employeeFromDb.Password = employee.Password;
                employeeFromDb.EmployeeNumber = employee.EmployeeNumber;
                employeeFromDb.Email = employee.Email;

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                db.SubmitChanges();
            }
        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal animalFromDb = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();

            if (animalFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return animalFromDb;
            }
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            var animalFromDb = db.Animals.Where(a => a.AnimalId == animalId).Single();

            foreach (KeyValuePair<int, string> item in updates)
            {
                int key = item.Key;
                switch (key)
                {
                    case 1:
                        animalFromDb.CategoryId = Convert.ToInt32(updates[1]);
                        break;
                    case 2:
                        animalFromDb.Name = updates[2];
                        break;
                    case 3:
                        animalFromDb.Age = Convert.ToInt32(updates[3]);
                        break;
                    case 4:
                        animalFromDb.Demeanor = updates[4];
                        break;
                    case 5:
                        animalFromDb.KidFriendly = Convert.ToBoolean(updates[5]);
                        break;
                    case 6:
                        animalFromDb.PetFriendly = Convert.ToBoolean(updates[6]);
                        break;
                    case 7:
                        animalFromDb.Weight = Convert.ToInt32(updates[7]);
                        break;
                    case 8:
                        animalFromDb.AnimalId = Convert.ToInt32(updates[8]);
                        break;
                }
            }

            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            animal = db.Animals.Where(a => a == animal).FirstOrDefault();
            db.Animals.DeleteOnSubmit(animal);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                db.SubmitChanges();
            }
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> animalsFromDB = db.Animals.Where(a => a.CategoryId == Convert.ToInt32(updates[1]) && a.Name == updates[2] &&
            a.Age == Convert.ToInt32(updates[3]) && a.Demeanor == updates[4] && a.KidFriendly == Convert.ToBoolean(updates[5]) && a.PetFriendly
            == Convert.ToBoolean(updates[6]) && a.Weight == Convert.ToInt32(updates[7]));

            return animalsFromDB;
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            Category categoryFromDb = db.Categories.Where(c => c.Name == categoryName).FirstOrDefault();

            if (categoryFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return categoryFromDb.CategoryId;
            }
        }
        
        internal static Room GetRoom(int animalId)
        {
            Room roomFromDb = db.Rooms.Where(r => r.AnimalId == animalId).FirstOrDefault();

            if (roomFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return roomFromDb;
            }
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            DietPlan dietPlanFromDb = db.DietPlans.Where(d => d.Name == dietPlanName).FirstOrDefault();

            if (dietPlanFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return dietPlanFromDb.DietPlanId;
            }
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            client = db.Clients.Where(a => a == client).FirstOrDefault();
            animal = db.Animals.Where(a => a == animal).FirstOrDefault();
            Adoption adoption = db.Adoptions.Where(a => a.ClientId == client.ClientId && a.AnimalId == animal.AnimalId).FirstOrDefault();
            animal.AdoptionStatus = "Adopted by: " + client.FirstName + " " + client.LastName;
            adoption.ApprovalStatus = "approved";
            adoption.PaymentCollected = true;

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                db.SubmitChanges();
            }
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            IQueryable<Adoption> pendingAdoptions = db.Adoptions.Where(a => a.ApprovalStatus == "Pending");
            return pendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            if (isAdopted == true)
            {
                adoption.ApprovalStatus = "approved";
                adoption.PaymentCollected = true;
            }
            else
            {
                adoption.ApprovalStatus = "Pending";
                adoption.PaymentCollected = true;
            }

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                db.SubmitChanges();
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            Adoption adoption = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).FirstOrDefault();
            db.Adoptions.DeleteOnSubmit(adoption);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                db.SubmitChanges();
            }
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            IQueryable<AnimalShot> shotsFromDB = db.AnimalShots.Where(a => a.Animal == animal);
            return shotsFromDB;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            Shot shotFromDB = db.Shots.Where(a => a.Name == shotName).FirstOrDefault();
            AnimalShot animalShot = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId && a.ShotId == shotFromDB.ShotId).FirstOrDefault();
            animalShot.DateReceived = DateTime.Now;

            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                db.SubmitChanges();
            }
        }
    }
}