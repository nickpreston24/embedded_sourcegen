using Hydro;

namespace hydro_app.Pages.Shared.Components
{
    public class HydroCounter : HydroComponent
    {
        public int Count { get; set; }

        public void Add()
        {
            Count++;
        }
    }
}