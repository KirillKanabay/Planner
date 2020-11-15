using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PlannerModel;
using UtilityLibraries;
namespace PlannerController
{
    /// <summary>
    /// Контроллер категории
    /// </summary>
    public class CategoryController
    {
        /// <summary>
        /// Список категорий
        /// </summary>
        public ObservableCollection<Category> Categories { get; private set; }

        public CategoryController()
        {
            Categories = GetCategories() ?? new ObservableCollection<Category>();
        }

        /// <summary>
        /// Создание нового контроллера категории
        /// </summary>
        /// <param name="name">Название категории</param>
        /// <param name="color">Цвет категории</param>
        public CategoryController(string name, string color)
        {
            Categories = GetCategories();
            #region Проверка условий
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Название категории не может быть пустым.");
            }
            if (!ColorExtensions.IsHexColor(color))
            {
                throw new ArgumentException("Неправильный формат цвета.");
            }
            #endregion

            var currentCategory = Categories.SingleOrDefault(item => item.Name == name);
            if (currentCategory == null)
            {
                AddCategory(new Category(name, color));
            }
            else
            {
                throw new ArgumentException("Такая категория уже существует.");
            }
            //Обновляем список категорий
            Categories = GetCategories();
        }
        /// <summary>
        /// Добавляет категорию в БД
        /// </summary>
        /// <param name="category"> Категория </param>
        private void AddCategory(Category category)
        {
            using (var context = new PlannerContext())
            {
                context.Categories.Add(category);
                context.SaveChanges();
            }
        }
        /// <summary>
        /// Возвращает список категорий из БД
        /// </summary>
        /// <returns>Список категорий</returns>
        private ObservableCollection<Category> GetCategories()
        {
            var categories = new ObservableCollection<Category>();
            using (var context = new PlannerContext())
            {
                foreach (var category in context.Categories)
                {
                    categories.Add(category);
                }
            }
            return categories;
        }

        public Category GetCategoryById(int id)
        {
            return Categories.SingleOrDefault(item => item.Id == id);
        }

        public Category GetCategoryByName(string name)
        {
            return Categories.SingleOrDefault(item => item.Name == name);
        }
    }
}
