using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Autor
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public List<Ksiazka> Ksiazki { get; set; }
    }

    public class Ksiazka
    {
        public int Id { get; set; }
        public string Tytul { get; set; }
        public int WydawnictwoId { get; set; }
        public Wydawnictwo Wydawnictwo { get; set; }
        public List<Autor> Autorzy { get; set; }
    }

    public class Wydawnictwo
    {
        public int Id { get; set; }
        public string Nazwa { get; set; }
        public List<Ksiazka> Ksiazki { get; set; }
    }

    public class BibliotekaContext : DbContext
    {
        public DbSet<Autor> Autorzy { get; set; }
        public DbSet<Ksiazka> Ksiazki { get; set; }
        public DbSet<Wydawnictwo> Wydawnictwa { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            using (var context = new BibliotekaContext())
            {
                Console.WriteLine("Dodaj Książke");

                Console.WriteLine("Podaj imie");
                string dodajimie = Console.ReadLine();

                Console.WriteLine("Podaj Nazwisko");
                string dodajnazwisko = Console.ReadLine();

                Console.WriteLine("Podaj książke");
                string dodajksiazke = Console.ReadLine();

                Console.WriteLine("Podaj wydawnictwo");
                string dodajwydawnictwo = Console.ReadLine();

                // Dodawanie przykładowych danych
                var wydawnictwo = new Wydawnictwo { Nazwa = dodajwydawnictwo };
                context.Wydawnictwa.Add(wydawnictwo);

                var autor = new Autor { Imie = dodajimie, Nazwisko = dodajnazwisko };
                context.Autorzy.Add(autor);

                var ksiazka = new Ksiazka
                {
                    Tytul = dodajksiazke,
                    Wydawnictwo = wydawnictwo,
                    Autorzy = new List<Autor> { autor }
                };
                context.Ksiazki.Add(ksiazka);

                context.SaveChanges();

                // Wyświetlanie danych z bazy
                var ksiazki = context.Ksiazki.Include(k => k.Wydawnictwo).Include(k => k.Autorzy).ToList();
                foreach (var k in ksiazki)
                {
                    Console.WriteLine($"Książka: {k.Tytul}, Wydawnictwo: {k.Wydawnictwo.Nazwa}, Autor: {string.Join(", ", k.Autorzy.Select(a => a.Imie + " " + a.Nazwisko))}");
                }

                // Usuwanie danych
                Console.WriteLine("Usuń Książke");
                Console.WriteLine("Podaj książke");
                string usunksiazke = Console.ReadLine();

                var usunksiazka = context.Ksiazki.FirstOrDefault(k => k.Tytul == usunksiazke);
                if (usunksiazka != null)
                {
                    // Usunięcie książki z kontekstu
                    context.Ksiazki.Remove(usunksiazka);

                    // Zapisanie zmian w bazie danych
                    context.SaveChanges();

                    Console.WriteLine("Książka została usunięta.");
                }
                else
                {
                    Console.WriteLine("Nie znaleziono książki o podanym tytule.");
                }

                foreach (var k in context.Ksiazki)
                {
                    Console.WriteLine($"Książka: {k.Tytul}, Wydawnictwo: {k.Wydawnictwo.Nazwa}, Autor: {string.Join(", ", k.Autorzy.Select(a => a.Imie + " " + a.Nazwisko))}");
                }



                // Edytowanie danych
                Console.WriteLine("Edytuj Książke");

                Console.WriteLine("Podaj tytuł książki do edycji (jak nie chcesz edytować to nic nie wpisuj)");
                string edytujksiazke = Console.ReadLine();

                var edytowanaKsiazka = context.Ksiazki.Include(k => k.Wydawnictwo).Include(k => k.Autorzy).FirstOrDefault(k => k.Tytul == edytujksiazke);
                if (edytowanaKsiazka != null)
                {
                    Console.WriteLine("Podaj nowy tytuł");
                    edytowanaKsiazka.Tytul = Console.ReadLine();

                    Console.WriteLine("Podaj nowe wydawnictwo");
                    string noweWydawnictwo = Console.ReadLine();
                    var wydawnictwoDoEdycji = context.Wydawnictwa.FirstOrDefault(w => w.Nazwa == noweWydawnictwo);
                    if (wydawnictwoDoEdycji == null)
                    {
                        wydawnictwoDoEdycji = new Wydawnictwo { Nazwa = noweWydawnictwo };
                        context.Wydawnictwa.Add(wydawnictwoDoEdycji);
                    }
                    edytowanaKsiazka.Wydawnictwo = wydawnictwoDoEdycji;

                    Console.WriteLine("Podaj nowe imię autora");
                    string noweImie = Console.ReadLine();
                    Console.WriteLine("Podaj nowe nazwisko autora");
                    string noweNazwisko = Console.ReadLine();
                    var autorDoEdycji = context.Autorzy.FirstOrDefault(a => a.Imie == noweImie && a.Nazwisko == noweNazwisko);
                    if (autorDoEdycji == null)
                    {
                        autorDoEdycji = new Autor { Imie = noweImie, Nazwisko = noweNazwisko };
                        context.Autorzy.Add(autorDoEdycji);
                    }
                    edytowanaKsiazka.Autorzy = new List<Autor> { autorDoEdycji };

                    context.SaveChanges();
                    Console.WriteLine("Książka została zedytowana.");
                }
                else
                {
                    Console.WriteLine("Nie znaleziono książki o podanym tytule.");
                }


                foreach (var k in context.Ksiazki)
                {
                    Console.WriteLine($"Książka: {k.Tytul}, Wydawnictwo: {k.Wydawnictwo.Nazwa}, Autor: {string.Join(", ", k.Autorzy.Select(a => a.Imie + " " + a.Nazwisko))}");
                }


                // Filtrowanie danych
                Console.WriteLine("Filtrowanie rekordów danych");
                Console.WriteLine("Podaj czym chcesz filtrować. Wybierz jedno z tych 3: (nazwisko autora, tytuł książki, nazwa wydawnictwa):");
                string kryterium = Console.ReadLine();

                Console.WriteLine("Podaj co chcesz wyszukać");
                string wartosc = Console.ReadLine();

                var filtrowaneKsiazki = context.Ksiazki.Include(k => k.Wydawnictwo).Include(k => k.Autorzy).AsQueryable();

                if (kryterium == "nazwisko autora")
                {
                    filtrowaneKsiazki = filtrowaneKsiazki.Where(k => k.Autorzy.Any(a => a.Nazwisko.Contains(wartosc)));
                }
                else if (kryterium == "tytuł książki")
                {
                    filtrowaneKsiazki = filtrowaneKsiazki.Where(k => k.Tytul.Contains(wartosc));
                }
                else if (kryterium == "nazwa wydawnictwa")
                {
                    filtrowaneKsiazki = filtrowaneKsiazki.Where(k => k.Wydawnictwo.Nazwa.Contains(wartosc));
                }
                else
                {
                    Console.WriteLine("Nieznane kryterium filtrowania.");
                }

                foreach (var k in filtrowaneKsiazki.ToList())
                {
                    Console.WriteLine($"Książka: {k.Tytul}, Wydawnictwo: {k.Wydawnictwo.Nazwa}, Autor: {string.Join(", ", k.Autorzy.Select(a => a.Imie + " " + a.Nazwisko))}");
                }

                Console.ReadLine();
            }
        }
    }
}