using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchUI
{
    public partial class AzureSearchPlayground : Form
    {
        public AzureSearchPlayground()
        {
            InitializeComponent();
        }

        private void ButtonIndex_Click(object sender, EventArgs e)
        {
            Article article = new Article
            {
                Title = InputTitle.Text,
                Category = InputCategory.Text,
                Text = InputText.Text
            };

            // index new item
            using (var serviceClient = new SearchServiceClient("clouddev", new SearchCredentials("5186266EED091A2D879713FCEF9C1F51")))
            {
                var actions = new IndexAction<Article>[]
                {
                    IndexAction.MergeOrUpload(article)
                };

                var batch = IndexBatch.New(actions);

                ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("articles");
                indexClient.Documents.Index(batch);
            }
        }

        private void ButtonSearch_Click(object sender, EventArgs e)
        {
            // get results from DB
            using (var indexClient = new SearchIndexClient("clouddev", "articles", new SearchCredentials("5186266EED091A2D879713FCEF9C1F51")))
            {
                var parameters = new SearchParameters()
                {
                };

                var results = indexClient.Documents.Search<Article>(textBox1.Text, parameters);

                ResultGrid.DataSource = results.Results.Select(x => new
                {
                    Score = x.Score,
                    Title = x.Document.Title,
                    Category = x.Document.Category,
                    Text = x.Document.Text
                }).OrderByDescending(x => x.Score).ToList();
            }
        }
    }
}
