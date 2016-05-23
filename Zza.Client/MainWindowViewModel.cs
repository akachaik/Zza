using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Zza.Client.ZzaServices;
using Zza.Entities;

namespace Zza.Client
{
    public class MainWindowViewModel : BindableBase
    {
        private ObservableCollection<Product> _products;
        private ObservableCollection<Customer> _customers;
        private ObservableCollection<OrderItemModel> _items = new ObservableCollection<OrderItemModel>();
        private Order _currentOrder = new Order();

        public MainWindowViewModel()
        {
            _currentOrder.OrderDate = DateTime.Now;
            _currentOrder.OrderStatusId = 1;
            SubmitOrderCommand = new DelegateCommand(OnSubmintOrder);
            AddOrderItemCommand = new DelegateCommand<Product>(OnAddItem);

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                LoadProductsAndCustomers();
            }

        }


        public ObservableCollection<Product> Products
        {
            get { return _products; }
            set { SetProperty(ref _products, value); }
        }


        public ObservableCollection<Customer> Customers
        {
            get { return _customers; }
            set { SetProperty(ref _customers, value); }
        }


        public ObservableCollection<OrderItemModel> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public Order CurrentOrder
        {
            get { return _currentOrder; }
            set { SetProperty(ref _currentOrder, value); }
        }


        private void OnAddItem(Product product)
        {
            var existingOrdetItem = _currentOrder.OrderItems.FirstOrDefault(oi => oi.ProductId == product.Id);
            var existingOrderItemModel = _items.FirstOrDefault(i => i.ProductId == product.Id);
            if (existingOrdetItem != null && existingOrderItemModel != null)
            {
                existingOrdetItem.Quantity++;
                existingOrderItemModel.Quantity++;
                existingOrdetItem.TotalPrice = existingOrdetItem.UnitPrice*existingOrdetItem.Quantity;
                existingOrderItemModel.TotalPrice = existingOrdetItem.TotalPrice;
            }
            else
            {
                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = 1,
                    UnitPrice = 9.99m,
                    TotalPrice = 9.99m
                };

                _currentOrder.OrderItems.Add(orderItem);
                Items.Add(new OrderItemModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = orderItem.Quantity
                });
            }
        }

        private async void LoadProductsAndCustomers()
        {
            var proxy = new ZzaServiceClient("NetTcpBinding_IZzaService");
            try
            {
                Products = await proxy.GetProductsAsync();
                Customers = await proxy.GetCustomersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                proxy.Close();
            }
        }


        private void OnSubmintOrder()
        {
            
        }

        public DelegateCommand<Product> AddOrderItemCommand { get; private set; }

        public DelegateCommand SubmitOrderCommand { get; set; }
    }
}
