using System;
using System.Collections.Generic;
using Gtk;
using static SQLiteCRUD;

namespace desafioford
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            new MainWindow(args);
            Application.Run();
        }
    }

    public class MainWindow
    {
        //Create widgets that will be accessed by other methods
        Entry entryPlaca = new Entry();
        Entry entryCor = new Entry();
        Gtk.TreeView tree = new Gtk.TreeView();
        Gtk.ListStore carrosListStore = new Gtk.ListStore(typeof(int), typeof(string), typeof(string), typeof(string));
        Entry entrySearch = new Entry();
        public MainWindow(string[] args) {
            
            //Create the Layout
            Window mainWin = new Window("Registro de frota");
            mainWin.Resize(800, 400);

            HBox mainBox = new HBox(homogeneous: false, spacing: 20);
            HBox searchBox = new HBox(homogeneous: false, spacing: 5);
            VBox addForm = new VBox(homogeneous: false, spacing: 5);
            VBox showData = new VBox(homogeneous: false, spacing: 5);
            HBox topLabel = new HBox(homogeneous: false, spacing: 5);
            HBox fieldA = new HBox(homogeneous: false, spacing: 5);
            HBox fieldB = new HBox(homogeneous: false, spacing: 5);
            HBox buttonBox = new HBox(homogeneous: false, spacing: 5);

            //Assemble the layout
            mainWin.Add(mainBox);

            mainBox.PackStart(addForm, expand: true, fill: true, padding: 0);
            addForm.PackStart(topLabel, expand: true, fill: true, padding: 0);
            addForm.PackStart(fieldA, expand: true, fill: true, padding: 0);
            addForm.PackStart(fieldB, expand: true, fill: true, padding: 0);
            addForm.PackStart(buttonBox, expand: true, fill: true, padding: 0);

            showData.PackStart(searchBox, expand: true, fill: true, padding: 0);
            mainBox.PackStart(showData, expand: true, fill: true, padding: 0);

            //Create Widgets
            //Create a label and put some text in it.
            Label descriptionLabel = new Label();
            descriptionLabel.Text = "Insira os dados do novo veículo.";

            //Add quit button
            Button btnQuit = new Button(Stock.Cancel);
            //Add OK button
            Button btnOk = new Button(Stock.Ok);
            //Add Delete button
            Button btnDelete = new Button(Stock.Delete);
            //Add Search button
            Button btnSearch = new Button(Stock.Find);

            //Add field Placa
            Label lblPlaca = new Label();
            lblPlaca.Text = "Placa";
            this.entryPlaca.Text = "ABC0123";

            //Add field Cor
            Label lblCor = new Label();
            lblCor.Text = "Cor";
            this.entryCor.Text = "Branco";

            //Add search label
            Label lblSearch = new Label();
            lblSearch.Text = "Buscar placa";

            //Update TreeView with data from database
            updateList();
            
            //Add widgets to the appropriate boxes
            bool expand = true;
            bool fill = true;
            uint padding = 0;

            topLabel.PackStart(descriptionLabel, expand, fill, padding);

            fieldA.PackStart(lblPlaca, expand, fill, padding);
            fieldA.PackStart(this.entryPlaca, expand, fill, padding);

            fieldB.PackStart(lblCor, expand, fill, padding);
            fieldB.PackStart(this.entryCor, expand, fill, padding);

            buttonBox.PackEnd(btnQuit, expand, fill, padding);
            buttonBox.PackEnd(btnOk, expand, fill, padding);

            searchBox.PackStart(lblSearch, expand, fill, padding);
            searchBox.PackStart(this.entrySearch, expand, fill, padding);
            searchBox.PackStart(btnSearch, expand, fill, padding);

            showData.PackStart(this.tree, expand, fill, padding);

            //Create tree columns
            this.tree.AppendColumn("id_carro", new Gtk.CellRendererText(), "text", 0);
            this.tree.AppendColumn("placa", new Gtk.CellRendererText(), "text", 1);
            this.tree.AppendColumn("cor", new Gtk.CellRendererText(), "text", 2);
            this.tree.AppendColumn("data", new Gtk.CellRendererText(), "text", 3);

            showData.PackStart(btnDelete, expand, fill, padding);

            //Create events
            mainWin.DeleteEvent += new DeleteEventHandler(delete_event);
            btnQuit.Clicked += new EventHandler(delete_event);
            btnOk.Clicked += new EventHandler(OnClickOK);
            btnDelete.Clicked += new EventHandler(OnClickDeleteEntry);
            btnSearch.Clicked += new EventHandler(OnClickSearch);


            //Show Everything
            mainWin.ShowAll();
        }

        private void OnClickSearch(object sender, EventArgs e)
        {
            string selectStatement = $"SELECT * FROM Frota WHERE placa LIKE '%{this.entrySearch.Text}%'";
            updateList(selectStatement);
        }

        private void OnClickDeleteEntry(object sender, EventArgs e)
        {
            TreeSelection selectedRow = this.tree.Selection;
            ITreeModel model;
            TreeIter iter;
            Carro carroToDelete;

            if (selectedRow.GetSelected(out model, out iter))
            {
                //If a row is selected in the TreeeView
                //get the id_carro and placa to create the carro object to be deleted
                int id_carroToDelete = Int32.Parse(carrosListStore.GetValue(iter,0).ToString());
                string placaToDelete = carrosListStore.GetValue(iter,1).ToString();
                carroToDelete = new Carro(placaToDelete);
                carroToDelete.id_carro = id_carroToDelete;
            }
            else
            {
                return;
            }

            //Dialog to confirm deletion
            var confirmationDialog = new MessageDialog
            (
                null,
                DialogFlags.Modal,
                MessageType.Question,
                ButtonsType.OkCancel,
                $"Deletando o carro de placa {carroToDelete.Placa}.\nConfirma?"
            );

            confirmationDialog.Show();
            int confirmation = confirmationDialog.Run();
            confirmationDialog.Dispose();

            if (confirmation == (int)ResponseType.Ok)
            {
                using (var conn = CreateConnection())
                {
                    DeleteData(conn, carroToDelete);
                }
            }
            updateList();
        }

        private void delete_event (object sender, EventArgs e)
        {
            Application.Quit();
        }
        private void OnClickOK (object sender, EventArgs e)
        {
            //Read entry fields
            string placa = this.entryPlaca.Text;
            string cor = this.entryCor.Text;

            //Create Car object
            Carro carro = new Carro(placa, cor);
            try
            {
                addNewCar(carro);
                updateList();
                //Clear entry fields
                this.entryPlaca.Text = "";
                this.entryCor.Text = "";
            }
            catch
            {
                var errorDialog = new MessageDialog
                (
                    null,
                    DialogFlags.DestroyWithParent,
                    MessageType.Error,
                    ButtonsType.Ok,
                    "Não foi possível adicionar. Verifique se o veículo já está cadastrado."
                );

                errorDialog.Show();
                errorDialog.Run();
                errorDialog.Dispose();
            }            
        }

        private void addNewCar(Carro carro)
        {
            using (var conn = CreateConnection())
            {
                InsertData(conn, carro);
            }
        }

        private void updateList(string selectStatement=null)
        {
            //Update values to be passed to TreeView
            //Store cars "id_carro", "placa", "cor", "data".
            carrosListStore.Clear();
            List<Carro> listCarros = new List<Carro>();

            using (var conn = CreateConnection())
            {
                if (selectStatement != null)
                {
                    listCarros = ReadData(conn, selectStatement);
                }
                else
                {
                    listCarros = ReadData(conn);
                }
            }
            foreach (Carro carro in listCarros)
            {
                carrosListStore.AppendValues(carro.id_carro, carro.Placa, carro.Cor, carro.Data);
            }
            //carrosListStore.AppendValues(1, "FHF3838", "Branco", "2020-11-22");
            tree.Model = carrosListStore;
        }
    }
}