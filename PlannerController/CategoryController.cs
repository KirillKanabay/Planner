using System;
using System.Collections.Generic;
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
        public List<Category> Categories { get; private set; }

        public CategoryController()
        {
            Categories = GetCategories();
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
            if (string.IsNullOrEmpty(color))
            {
                color = ColorLibrary.GenerateRGBColor();
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
        private List<Category> GetCategories()
        {
            var categories = new List<Category>();
            using (var context = new PlannerContext())
            {
                foreach (var category in context.Categories)
                {
                    categories.Add(category);
                }
            }
            return categories;
        }
    }
}
