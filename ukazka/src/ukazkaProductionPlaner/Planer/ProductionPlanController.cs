using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using TcOpen.Inxton.Data;
using ukazkaPlc;
using TcOpen.Inxton.RepositoryDataSet;
using System;

namespace ukazkaProductionPlaner.Planer
{
    public class ProductionPlanController : INotifyPropertyChanged
    {
        public ProductionPlanController(RepositoryDataSetHandler<ProductionItem> productionSetData, string configName, IRepository<PlainProcessData> repositorySource)
        {
            DataHandler = productionSetData;
            ConfigName = configName;
            RepositorySource = repositorySource;
            var items = RepositorySource.Queryable.Select(p => p._EntityId).ToList();
            RecipeCollection = new ObservableCollection<string>(items);
        }

        private ProductionItem currentItem;

        /// <summary>
        /// Gets or sets current item.
        /// </summary>
        public ProductionItem CurrentItem
        {
            get => currentItem;
            set
            {
                if (currentItem == value)
                {
                    return;
                }

                currentItem = value;

                OnPropertyChanged(nameof(CurrentItem));
            }
        }

        internal void DeleteAll()
        {
            CurrentProductionSet.Items.Clear();
            OnPropertyChanged("Items");
            OnPropertyChanged("CurrentProductionSet");

        }

        /// <summary>
        /// Gets current Production set.
        /// </summary>
        public EntitySet<ProductionItem> CurrentProductionSet { get; set; } = new EntitySet<ProductionItem>();

        /// <summary>
        /// Gets production of this 
        /// </summary>
        protected RepositoryDataSetHandler<ProductionItem> DataHandler { get; }
        public string ConfigName { get; set; }
        public IRepository<PlainProcessData> RepositorySource { get; private set; }

        public bool ProductionPlanCompleted
        {
            get => productionPlanCompleted;
            set
            {
                if (productionPlanCompleted == value)
                {
                    return;
                }

                productionPlanCompleted = value;
                OnPropertyChanged(nameof(ProductionPlanCompleted));
            }
        }
        public bool ProductionPlanEmpty
        {
            get => productionPlanEmpty;
            set
            {
                if (productionPlanEmpty == value)
                {
                    return;
                }

                productionPlanEmpty = value;
                OnPropertyChanged(nameof(ProductionPlanEmpty));
            }
        }


        private readonly ProductionItem AllCompetedItem = new ProductionItem() { Status = EnumItemStatus.AllCompleted };
        private readonly ProductionItem EmptyItem = new ProductionItem() { Status = EnumItemStatus.None };
        private bool productionPlanCompleted;
        private bool productionPlanEmpty;

        /// <summary>
        /// When overriden performs update of <see cref="CurrentItem"/>.
        /// </summary>
        public void RefreshItems(out ProductionItem itm)
        {
            LoadDataSet(ConfigName);

            var item = CurrentProductionSet.Items.Where(p => p.Status == EnumItemStatus.Required 
                                                        || p.Status == EnumItemStatus.Active).FirstOrDefault();
         
            if (item != null)
            {
                CurrentItem = item;
                CurrentItem.ActualCount++;

                if (CurrentItem.ActualCount < CurrentItem.RequiredCount)
                    CurrentItem.Status = EnumItemStatus.Active;
                else if (CurrentItem.ActualCount >= CurrentItem.RequiredCount)
                    CurrentItem.Status = EnumItemStatus.Done;
            }
            else if (CurrentProductionSet.Items.Count() == 0) CurrentItem = EmptyItem;
            else if (CurrentProductionSet.Items.Count() > 0) CurrentItem = AllCompetedItem;

            itm = CurrentItem;
            ProductionPlanCompleted = CurrentItem.Status == EnumItemStatus.AllCompleted;
            ProductionPlanEmpty = CurrentItem.Status == EnumItemStatus.None && CurrentProductionSet.Items.Count() == 0;

            SaveDataSet(ConfigName);
            OnPropertyChanged(nameof(CurrentProductionSet));

        }
        /// <summary>
        /// Set all current counters to 0 (clear counters)
        /// </summary>
        public void Reinit()
        {
            CurrentProductionSet.Items.ToList().ForEach(p => p.ActualCount = 0);
            

        }
       
        public void RefreshSourceRecipeList()
        {
            var items = RepositorySource.Queryable.Select(p => p._EntityId).ToList();
            RecipeCollection = new ObservableCollection<string>(items);
        }
        /// <summary>
        /// Loads items set from the repository to this controller.
        /// </summary>
        /// <param name="setid">set id.</param>
        public void LoadDataSet(string setid)
        {
            var result = DataHandler.Repository.Queryable.FirstOrDefault(p => p._EntityId == setid);

            if (result == null)
            {
                DataHandler.Create(setid, CurrentProductionSet);
            }

            CurrentProductionSet = DataHandler.Read(setid);
        }

        /// <summary>
        /// Saves items set from this controller to the repository.
        /// </summary>
        /// <param name="setId">Instrucion set id.</param>
        public void SaveDataSet(string setId)
        {
          
            if (! DataHandler.Repository.Queryable.Where(p => p._EntityId == setId).Any())
            {
                DataHandler.Create(setId, CurrentProductionSet);
            }
            DataHandler.Update(setId, CurrentProductionSet);


        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ObservableCollection<string> RecipeCollection { get; private set; }

    }
}
