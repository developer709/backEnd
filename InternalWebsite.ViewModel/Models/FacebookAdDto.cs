using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebsite.ViewModel.Models
{
    public class FacebookAdDto
    {
        public class DateTimeResult
        {
            public string StartTime { get; set; }
            public string EndTime { get; set; }
            public string ExtendTime { get; set; }
        }
        public class PictureData
        {
            public int Height { get; set; }
            public bool IsSilhouette { get; set; }
            public string Url { get; set; }
            public int Width { get; set; }
        }

        public class Picture
        {
            public PictureData Data { get; set; }
        }

        public class PageData
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public Picture Picture { get; set; }
        }

        public class PagingCursors
        {
            public string Before { get; set; }
            public string After { get; set; }
        }

        public class Paging
        {
            public PagingCursors Cursors { get; set; }
        }

        public class RootObject
        {
            public List<PageData> Data { get; set; }
            public Paging Paging { get; set; }
        }
        public class Place
        {
            public string Description { get; set; }
            public string PlaceId { get; set; }
            public string Reference { get; set; }
        }

        public class Position
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
        }

        public class CustomLocation
        {
            public Place Place { get; set; }
            public Position Position { get; set; }
            public int Radius { get; set; }
        }

    }
}
