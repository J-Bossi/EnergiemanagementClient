namespace Ork.Energy.Domain.DomainModelService
{
  public interface IDistributor {
    /// <summary>
    /// There are no comments for Property Id in the schema.
    /// </summary>
    int Id { get; set; }

    /// <summary>
    /// There are no comments for Property Name in the schema.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// There are no comments for Property Comment in the schema.
    /// </summary>
    string Comment { get; set; }

    /// <summary>
    /// There are no comments for Property IsMainDistributor in the schema.
    /// </summary>
    bool IsMainDistributor { get; set; }

    /// <summary>
    /// There are no comments for Readings in the schema.
    /// </summary>
    global::System.Data.Services.Client.DataServiceCollection<Reading> Readings { get; set; }

    /// <summary>
    /// There are no comments for Room in the schema.
    /// </summary>
    Room Room { get; set; }
  }
}