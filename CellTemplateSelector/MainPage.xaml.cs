using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Telerik.Windows.Controls;

namespace CellTemplateSelector
{
    public partial class MainPage 
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new ViewModel();
            
        }
    }

    public class ViewModel : ViewModelBase
    {
        public ViewModel()
        {
            Durations = new DeepObservableCollection<DurationEntity>
            {
                new DurationEntity
                {
                    StartEventLocalDateTime = new DateTime(2019, 1,1),
                    EndEventLocalDateTime  = new DateTime(2019, 12, 1)
                },
                new DurationEntity
                {
                    StartEventLocalDateTime = new DateTime(2019, 1,1),
                    EndEventLocalDateTime  = new DateTime(2019, 11, 1)
                },

            };
            UpdateOverlapped();

            Durations.CollectionChanged += (sender, args) =>
            {
                UpdateOverlapped();
            };

            Durations.CollectionOrMemberPropertyChanged += (sender, args) =>
            {
                UpdateOverlapped();
            };

        }

        void UpdateOverlapped()
        {
            foreach (var d in Durations)
            {
                d.Overlapped = Overlapped(Durations, d); 
            }
        }

        bool Overlapped(IEnumerable<DurationEntity> items, DurationEntity duration)
        {
            return items.Any(od =>
                !ReferenceEquals(od, duration) &&
                duration.EndEventLocalDateTime > od.StartEventLocalDateTime &&
                duration.EndEventLocalDateTime < od.EndEventLocalDateTime);
        }

        public DeepObservableCollection<DurationEntity> Durations { get; set; }
    }

    public class DurationEntity : ViewModelBase //, IEditableObject
    {
        DateTime? _startEventLocalDateTime;

        bool _overlapped;
//        DateTime? originalStart;
//        DateTime? originalEnd;

        public DateTime? StartEventLocalDateTime
        {
            get { return _startEventLocalDateTime; }
            set
            {
                if (_startEventLocalDateTime != value)
                {
                    _startEventLocalDateTime = value;
                    OnPropertyChanged(nameof(StartEventLocalDateTime));
                }
            }
        }

        DateTime? _endEventLocalDateTime;

        public DateTime? EndEventLocalDateTime
        {
            get { return _endEventLocalDateTime; }
            set
            {
                if (_endEventLocalDateTime != value)
                {
                    _endEventLocalDateTime = value;
                    OnPropertyChanged(nameof(EndEventLocalDateTime));
                }
            }
        }

        public bool Overlapped
        {
            get { return _overlapped; }
            set
            {
                if (_overlapped != value)
                {
                    _overlapped = value;
                    OnPropertyChanged(nameof(Overlapped));
                    Console.WriteLine($"Overlapped now {_overlapped}");
                    // can't see the point of BeginEdit/EndEdit as IEditable has no events
//                    BeginEdit();
//                    EndEdit();
                }
            }
        }

//        private bool IsEditing { get; set; }
//
//        public void BeginEdit()
//        {
//            if (IsEditing)
//            {
//                return;
//            }
//
//            IsEditing = true;
//
//            this.originalStart = this.StartEventLocalDateTime;
//            this.originalEnd = this.EndEventLocalDateTime;
//        }
//
//        public void CancelEdit()
//        {
//            this.StartEventLocalDateTime = this.originalStart;
//            this.EndEventLocalDateTime = this.originalEnd;
//        }
//
//        public void EndEdit()
//        {
//            if (!IsEditing)
//            {
//                return;
//            }
//
//            IsEditing = false;
//        }
    }

    public delegate void DeepObservableCollectionChangedEventHandler(object sender, DeepObservableCollectionChangedEvent args);
    public class DeepObservableCollectionChangedEvent : EventArgs
    {
        public PropertyChangedEventArgs PropertyChangedEventArgs { get; }

        public DeepObservableCollectionChangedEvent(PropertyChangedEventArgs e)
        {
            PropertyChangedEventArgs = e;
        }
    }
    public class DeepObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        // dont fire the base event as that causes problems 
        public event DeepObservableCollectionChangedEventHandler CollectionOrMemberPropertyChanged;

        public DeepObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        public DeepObservableCollection(IEnumerable<T> pItems) : this()
        {
            foreach (var item in pItems)
            {
                Add(item);
            }
        }

        void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CollectionOrMemberPropertyChanged?.Invoke(sender, new DeepObservableCollectionChangedEvent(e));
        }
    }
}
