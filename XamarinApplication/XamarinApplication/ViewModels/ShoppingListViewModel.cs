using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Models;

namespace XamarinApplication.ViewModels
{
    public class ShoppingListViewModel : BindableObject
    {
        ObservableCollection<ShoppingItem> _items;

        public ShoppingListViewModel()
        {
            LoadShoppingList();
        }

        public ObservableCollection<ShoppingItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        void LoadShoppingList()
        {
            var items = new List<ShoppingItem>
            {
                new ShoppingItem { Name = "Breads and pastries", Icon = "request", Color = Color.DeepPink,
                    Items = new List<ShoppingDetailItem>
                    {
                        new ShoppingDetailItem { Name = "Grain bread" },
                        new ShoppingDetailItem { Name = "Rolls" },
                        new ShoppingDetailItem { Name = "Cookies" },
                        new ShoppingDetailItem { Name = "Chocolate cake" },
                        new ShoppingDetailItem { Name = "Fruit cake" },
                        new ShoppingDetailItem { Name = "Baguette", IsLatest = true },
                    } },
                new ShoppingItem { Name = "Fish", Icon = "attachment", Color = Color.Orange,
                    Items = new List<ShoppingDetailItem>
                    {
                        new ShoppingDetailItem { Name = "Swordfish" },
                        new ShoppingDetailItem { Name = "Tuna" },
                        new ShoppingDetailItem { Name = "Salmon", IsLatest = true },
                    } }
            };
            Items = new ObservableCollection<ShoppingItem>(items);
        }
    }
}
