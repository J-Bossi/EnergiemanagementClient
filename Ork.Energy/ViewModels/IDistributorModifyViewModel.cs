using System.Collections.Generic;
using Ork.Energy.Domain.DomainModelService;

namespace Ork.Energy.ViewModels
{
  public interface IDistributorModifyViewModel {
    Distributor Model { get; }
    ReadingAddViewModel ReadingAddVm { get; set; }
    string Name { get; set; }
    string Comment { get; set; }
    bool IsMainDistributor { get; set; }
    IList<ReadingViewModel> Readings { get; }
    Room Room { get; set; }
    IEnumerable<Room> Rooms { get; }
    void AddNewReading(object dataContext);
    void DeleteReading(object dataContext);
  }
}