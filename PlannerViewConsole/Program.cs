using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PlannerController;
using PlannerModel;
using UtilityLibraries;

namespace PlannerViewConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var categoryController = new CategoryController();
            var taskController = new TaskController();
            var priorityController = new PriorityController();
            Console.WriteLine("=====Список приоритетов=====");
            PrintPriority(priorityController.Priorities);
            Console.WriteLine("=====Список категорий=====");
            PrintCategory(categoryController.Items);
            Console.WriteLine("=====Создание задачи=====");
            for (int i = 0; i < 1; i++)
            {
                CreateTask(taskController);
            }
            Console.WriteLine("=====Список задач=====");
            PrintTask(taskController.Tasks);
            Console.ReadLine();
        }

        static void CreateTask(TaskController task)
        {
            Console.Write("Введите название задачи:");
            string name = Console.ReadLine();
            Console.Write("Введите дату начала:");
            DateTime st = DateTime.Parse(Console.ReadLine());
            Console.Write("Введите дату конца:");
            DateTime et = DateTime.Parse(Console.ReadLine());
            Console.Write("Введите id категории:");
            int ci = int.Parse(Console.ReadLine());
            Console.Write("Введите id приоритета:");
            int pi = int.Parse(Console.ReadLine());
            task.AddTask(name,st,et,pi,ci);
        }
        static void PrintCategory(List<Category> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine($"id {item.Id} Name:{item.Name}");   
            }
        }
        static void PrintPriority(List<Priority> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine($"id {item.Id} Name:{item.Name}");
            }
        }
        static void PrintTask(List<Task> list)
        {
            foreach (var item in list)
            {
                Console.WriteLine($"id {item.Id} Name:{item.Name} CD:{item.CreationDate} ST:{item.StartTime} ET:{item.EndTime} Category:{item.Category.Name} Priority:{item.Priority.Name}");
            }
        }
        static async void Clear()
        {
            using (var db = new PlannerContext())
            {
                await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM dbo.Categories DBCC CHECKIDENT('dbo.Categories', RESEED, 0)");
                await db.Database.ExecuteSqlCommandAsync(@"DELETE FROM dbo.Priorities DBCC CHECKIDENT('dbo.Priorities', RESEED, 0)");
                db.Categories.Add(new Category("Без категории", "ffffff"));
                db.Priorities.Add(new Priority("Без приоритета", "ffffff"));
                db.SaveChanges();
            }
        }
    }
}
