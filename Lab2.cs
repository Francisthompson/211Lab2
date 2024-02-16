using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EmployeeManagement
{
    class Employee
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public Employee()
        {
        }

        public Employee(string id, string name, string address)
        {
            Id = id;
            Name = name;
            Address = address;
        }
    }

    class Salaried : Employee
    {
        public long Sin { get; set; }
        public double Salary { get; set; }

        public Salaried()
        {
        }

        public Salaried(string id, string name, string address, long sin)
            : base(id, name, address)
        {
            Sin = sin;
        }
    }

    class PartTime : Employee
    {
        public long Sin { get; set; }
        public double Rate { get; set; }

        public PartTime()
        {
        }

        public PartTime(string id, string name, string address, long sin)
            : base(id, name, address)
        {
            Sin = sin;
        }
    }

    class Wages : Employee
    {
        public long Sin { get; set; }
        public double Rate { get; set; }

        public Wages()
        {
        }

        public Wages(string id, string name, string address, long sin)
            : base(id, name, address)
        {
            Sin = sin;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Employee> employees = LoadEmployees("res/employees.txt");
            if (employees == null)
            {
                Console.WriteLine("Failed to load employee data.");
                return;
            }

            double averageWeeklyPay = CalculateAverageWeeklyPay(employees);
            Console.WriteLine($"Average weekly pay for all employees: {averageWeeklyPay:C}");

            var highestWage = CalculateHighestWeeklyPayForWages(employees);
            Console.WriteLine($"Highest weekly pay for wage employees: {highestWage.Value:C}, Name: {highestWage.Key.Name}");

            var lowestSalary = CalculateLowestSalaryForSalaried(employees);
            Console.WriteLine($"Lowest salary for salaried employees: {lowestSalary.Value:C}, Name: {lowestSalary.Key.Name}");

            var percentageByCategory = CalculatePercentageByCategory(employees);
            Console.WriteLine($"Percentage of employees in each category:");
            foreach (var kvp in percentageByCategory)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value:P}");
            }
        }

        static List<Employee> LoadEmployees(string filePath)
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] data = line.Split(',');
                        string id = data[0];
                        string name = data[1];
                        string address = data[2];
                        long sin = long.Parse(data[3]);

                        if (id.StartsWith("0") || id.StartsWith("1") || id.StartsWith("2") || id.StartsWith("3") || id.StartsWith("4"))
                        {
                            double salary = double.Parse(data[4]);
                            employees.Add(new Salaried(id, name, address, sin) { Salary = salary });
                        }
                        else if (id.StartsWith("5") || id.StartsWith("6") || id.StartsWith("7"))
                        {
                            double rate = double.Parse(data[4]);
                            employees.Add(new Wages(id, name, address, sin) { Rate = rate });
                        }
                        else if (id.StartsWith("8") || id.StartsWith("9"))
                        {
                            double rate = double.Parse(data[4]);
                            employees.Add(new PartTime(id, name, address, sin) { Rate = rate });
                        }
                    }
                }
                return employees;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading employees: {ex.Message}");
                return null;
            }
        }

        static double CalculateAverageWeeklyPay(List<Employee> employees)
        {
            double totalPay = 0;
            foreach (var employee in employees)
            {
                if (employee is Salaried salaried)
                {
                    totalPay += salaried.Salary;
                }
                else if (employee is Wages wages)
                {
                    totalPay += wages.Rate * 40; // Assuming 40 hours is the regular work week
                }
                else if (employee is PartTime partTime)
                {
                    totalPay += partTime.Rate * 20; // Assuming 20 hours for part-time work
                }
            }
            return totalPay / employees.Count;
        }

        static KeyValuePair<Wages, double> CalculateHighestWeeklyPayForWages(List<Employee> employees)
        {
            var wageEmployees = employees.OfType<Wages>();
            if (!wageEmployees.Any())
            {
                return new KeyValuePair<Wages, double>(null, 0);
            }
            var highestPay = wageEmployees.Max(w => w.Rate * Math.Min(40, w.Rate)); // Assuming 40 hours is the regular work week
            var employee = wageEmployees.First(w => (w.Rate * Math.Min(40, w.Rate)) == highestPay);
            return new KeyValuePair<Wages, double>(employee, highestPay);
        }

        static KeyValuePair<Salaried, double> CalculateLowestSalaryForSalaried(List<Employee> employees)
        {
            var salariedEmployees = employees.OfType<Salaried>();
            if (!salariedEmployees.Any())
            {
                return new KeyValuePair<Salaried, double>(null, 0);
            }
            var lowestSalary = salariedEmployees.Min(s => s.Salary);
            var employee = salariedEmployees.First(s => s.Salary == lowestSalary);
            return new KeyValuePair<Salaried, double>(employee, lowestSalary);
        }

        static Dictionary<string, double> CalculatePercentageByCategory(List<Employee> employees)
        {
            int totalEmployees = employees.Count;
            int salariedCount = employees.OfType<Salaried>().Count();
            int wagesCount = employees.OfType<Wages>().Count();
            int partTimeCount = employees.OfType<PartTime>().Count();

            var percentages = new Dictionary<string, double>();
            percentages["Salaried"] = (double)salariedCount / totalEmployees;
            percentages["Wages"] = (double)wagesCount / totalEmployees;
            percentages["PartTime"] = (double)partTimeCount / totalEmployees;

            return percentages;
        }
    }
}
