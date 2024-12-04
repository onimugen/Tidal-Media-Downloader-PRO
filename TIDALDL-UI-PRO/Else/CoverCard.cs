using AIGS.Common;
using AIGS.Helper;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static AIGS.Helper.HttpHelper;

namespace TIDALDL_UI.Else
{
    public class CoverCard 
    {
        public string ImgUrl { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Url { get; set; }
        
        public static async Task<ObservableCollection<CoverCard>> GetList()
        {
            try
            {
                Result result = await HttpHelper.GetOrPostAsync("https://raw.githubusercontent.com/onimugen/Tidal-Media-Downloader-PRO/refs/heads/master/todaycards.json");
                if(result.sData.IsNotBlank())
                {
                    ObservableCollection<CoverCard> pList = JsonHelper.ConverStringToObject<ObservableCollection<CoverCard>>(result.sData);
                    return pList;
                }
            }
            catch { }
            return GetDefaultList();
        }

        private static ObservableCollection<CoverCard> GetDefaultList()
        {
            CoverCard card1 = new CoverCard()
            {
                ImgUrl = "https://resources.tidal.com/images/3a4bd2c8/8e5d/4836/b5a8/6c4c2c929c5c/320x320.jpg",
                Title = "Servitude",
                SubTitle = "The Black Dahlia Murder",
                Url = "https://listen.tidal.com/album/198615449",
            };
            CoverCard card2 = new CoverCard()
            {
                ImgUrl = "https://resources.tidal.com/images/c3a3f59c/0c9d/4e5c/a77f/5b9e2d52e08d/320x320.jpg",
                Title = "Elegy",
                SubTitle = "Shadow Of Intent",
                Url = "https://listen.tidal.com/album/213217232",
            };
            CoverCard card3 = new CoverCard()
            {
                ImgUrl = "https://resources.tidal.com/images/8a49d6c9/c42e/49b3/9fb8/de6e5fd0f7a0/320x320.jpg",
                Title = "アイドル",
                SubTitle = "Yoasobi",
                Url = "https://listen.tidal.com/album/275856769",
            };
            ObservableCollection<CoverCard> pCards = new ObservableCollection<CoverCard>();
            pCards.Add(card1);
            pCards.Add(card2);
            pCards.Add(card3);

            string sjson = JsonHelper.ConverObjectToString<ObservableCollection<CoverCard>>(pCards);
            FileHelper.Write(sjson, true, "./covercards.json");

            return pCards;
        }
    }
}
