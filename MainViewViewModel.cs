using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITraningLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITrainingUI
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        public DelegateCommand SelectCommand { get; }
        public DelegateCommand SaveCommand { get; } 
        public DelegateCommand SelectPipes { get; }
        public DelegateCommand SelectWalls { get; }
        public DelegateCommand SelectDoors { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData=commandData;
            SaveCommand = new DelegateCommand(OnSaveCommand);
            SelectPipes = new DelegateCommand(OnSelectPipes);
            SelectWalls = new DelegateCommand(OnSelectWalls);
            SelectDoors = new DelegateCommand(OnSelectDoors);
        }
        public event EventHandler HideRequest;
        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ShowRequest;
        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }
        private void OnSaveCommand()
        {
            throw new NotImplementedException();
        }

        private void OnSelectPipes()
        {
            RaiseHideRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var Pipes = new FilteredElementCollector(doc)
                .OfClass(typeof(Pipe))
                .Cast<Pipe>()
                .ToList();

            TaskDialog.Show("Количество труб", $"Общее количество труб {Pipes.Count.ToString()}");
            RaiseShowRequest();
        }

        private void OnSelectWalls()
        {
            RaiseHideRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            IList<Element> walls = new FilteredElementCollector(doc)
                                .OfCategory(BuiltInCategory.OST_Walls)
                                .WhereElementIsNotElementType()
                                .ToElements();

            double vol = 0;
            foreach (Element element in walls)
            {
                Wall wallValue = element as Wall;
                if (element is Wall)
                {
                    Parameter volParameter = element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED);
                    if (volParameter.StorageType == StorageType.Double)
                    {
                        double volValue = UnitUtils.ConvertFromInternalUnits(volParameter.AsDouble(), UnitTypeId.CubicMeters);
                        vol += volValue;
                    }
                }
            }
            TaskDialog.Show("Общий объем стен", $"Общий объем стен {vol.ToString()}");
            RaiseShowRequest();
        }

        private void OnSelectDoors()
        {
            RaiseHideRequest();

            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument; 
            Document doc = uidoc.Document;

            ElementClassFilter familyInstanceFilter = new ElementClassFilter(typeof(FamilyInstance));
            ElementCategoryFilter doorsCategoryfilter = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            LogicalAndFilter doorInstancesFilter = new LogicalAndFilter(familyInstanceFilter, doorsCategoryfilter);
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ICollection<ElementId> Doors = collector.WherePasses(doorInstancesFilter).ToElementIds();

            TaskDialog.Show("Количество дверей", $"Общее количество дверей  {Doors.Count}");

            RaiseShowRequest();
        }
    }
}

