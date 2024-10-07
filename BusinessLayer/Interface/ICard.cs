
namespace MTCG.BusinessLayer.Interface
{
    internal interface ICard
    {

        string Name { get; set; }
        int Damage { get; set; }
        ElementType ElementType { get; set; }
        string ToString();

    }
}
