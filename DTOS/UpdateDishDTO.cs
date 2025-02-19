namespace FoodMenuApi.DTOS
{
    public class UpdateDishDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public List<int> IngredientIds { get; set; } = new List<int>();
    }
}
