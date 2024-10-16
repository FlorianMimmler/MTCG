
namespace MTCG.BusinessLayer.Interface
{
    internal interface ICard
    {
        string Id { get; set; }
        string Name { get; set; }
        int Damage { get; set; }
        ElementType ElementType { get; set; }
        string ToString();

    }
}
