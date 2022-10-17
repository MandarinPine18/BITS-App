﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordPressPCL;

namespace BITS_App.Models
{
    internal class Article
    {
        // JSON deserialization classes
        protected class Post {
            public int id { get; set; }
            public DateTime date { get; set; }
            public DateTimeOffset date_gmt { get; set; }
            public Dictionary<string, string> title { get; set; }
            public CustomFields custom_fields { get; set; }
        }

        protected class CustomFields {
            public string[] writer { get; set; }
        }

        // fields
        protected WordPressClient client;
        protected int id;
        protected WordPressPCL.Models.Post post;
        protected string raw;
        protected Post postJson;
        protected List<WordPressPCL.Models.MediaItem> medias;

        // constructor
        public Article(int id)
        {
            client = new WordPressClient("https://gwhsnews.org/wp-json/");
            this.id = id;

            raw = "{}";
            client.HttpResponsePreProcessing = (response) => raw = response;

            var task = client.Posts.GetByIDAsync(id);
            task.Wait();
            post = task.Result;

            postJson = JsonConvert.DeserializeObject<Post>(raw);

            var pictask = client.Media.GetAllAsync();
            pictask.Wait();
            medias = (List<WordPressPCL.Models.MediaItem>)pictask.Result;
        }

        // methods
        public string Title => post.Title.Rendered;

        public string Authors => String.Join(", ", postJson.custom_fields.writer);

        public string Image => medias[0].Link.ToString();
    }
}