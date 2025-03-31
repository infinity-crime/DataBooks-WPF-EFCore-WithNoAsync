using DataBooks.Commands;
using DataBooks.Models.BookItems;
using DataBooks.Models.Context;
using DataBooks.Models.DTO;
using DataBooks.Models.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Microsoft.Xaml.Behaviors.Media;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using System.Net;

namespace DataBooks.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // Команды для View
        public ICommand AddBookCommand { get; }
        public ICommand AddBookReviewCommand { get; }
        public ICommand SortBooksCommand { get; }

        // Переменные для временного хранения при заполнении данных о книге
        private string? _title;
        private string? _description;
        private string? _publishedOn;
        private string? _publisher;
        private string? _price;
        private string? _authorName;
        private string? _tag;

        // Формат вывода книг на View (Data Transfer Object)
        private BookListDto? _selectedBook;

        // Временные переменные для хранения информации при добавлении отзыва
        private string? _reviewText;
        private int? _selectedRating;

        private string? _selectedSortOption;

        // Коллекция для загрузки книг в память из БД
        private ObservableCollection<BookListDto> _bookListDtos;

        // Множество вариантов оценки
        private List<int> _reviewMarks;

        private List<string> _sortingOptions;

        public List<int> ReviewMarks
        {
            get => _reviewMarks;
            set
            {
                _reviewMarks = value;
                OnPropertyChanged();
            }
        }

        public List<string> SortingOptions
        {
            get => _sortingOptions;
            set
            {
                _sortingOptions = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BookListDto> BookListDtos
        {
            get => _bookListDtos;
            set
            {
                _bookListDtos = value;
                OnPropertyChanged();
            }
        }

        public string? Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public string? PublishedOn
        {
            get => _publishedOn;
            set
            {
                _publishedOn = value;
                OnPropertyChanged();
            }
        }

        public string? Publisher
        {
            get => _publisher;
            set
            {
                _publisher = value;
                OnPropertyChanged();
            }
        }

        public string? Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }

        public string? AuthorName
        {
            get => _authorName;
            set
            {
                _authorName = value;
                OnPropertyChanged();
            }
        }

        public string? Tag
        {
            get => _tag;
            set
            {
                _tag = value;
                OnPropertyChanged();
            }
        }

        public BookListDto? SelectedBook
        {
            get => _selectedBook;
            set
            {
                _selectedBook = value;
                OnPropertyChanged();
            }
        }

        public string? ReviewText
        {
            get => _reviewText;
            set
            {
                _reviewText = value;
                OnPropertyChanged();
            }
        }

        public int? SelectedRating
        {
            get => _selectedRating;
            set
            {
                _selectedRating = value;
                OnPropertyChanged();
            }
        }

        public string? SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            ReviewMarks = new List<int> { 1, 2, 3, 4, 5 };
            SortingOptions = new List<string> { "Наивысший рейтинг", "Сначала новые", "Сначала дешевые", 
            "Сначала дорогие", "По названию"};

            AddBookCommand = new RelayCommand(_ => AddBook());
            SortBooksCommand = new RelayCommand(_ => SortBooks());
            AddBookReviewCommand = new RelayCommand(_ => AddBookReview());

            LoadBooks();
        }

        private void AddBook()
        {
            if(BaseValidation(out string errors) > 0)
            {
                MessageBox.Show(errors, "Ошибки ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var author = db.Authors.FirstOrDefault(a => a.Name == AuthorName);
                    if (author == null)
                    {
                        author = new Author { Name = AuthorName! };
                        db.Authors.Add(author);
                    }

                    var tag = db.Tags.FirstOrDefault(t => t.TagId == Tag);
                    if (tag == null)
                    {
                        tag = new Tag { TagId = Tag! };
                        db.Tags.Add(tag);
                    }

                    var newBook = new Book
                    {
                        Title = Title!,
                        Description = Description!,
                        PublishedOn = DateTime.Parse(PublishedOn!),
                        Publisher = Publisher!,
                        Price = Decimal.Parse(Price!),
                    };

                    newBook.AuthorsLink.Add(author);
                    newBook.Tags.Add(tag);

                    db.Books.Add(newBook);
                    db.SaveChanges();
                }

                Title = Description = PublishedOn = Publisher = Price = AuthorName = Tag = null;

                SelectedSortOption = null;

                LoadBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения в базу данных: {ex.Message}\n{ex.InnerException?.Message}", "Error Database", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddBookReview()
        {
            if (SelectedBook == null && !SelectedRating.HasValue && string.IsNullOrEmpty(ReviewText))
            {
                MessageBox.Show("Вы не заполнили все поля!", "Отмена", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var newReview = new Review
                    {
                        BookId = SelectedBook!.BookId,
                        Comment = ReviewText!,
                        NumStars = SelectedRating!.Value
                    };

                    db.Reviews.Add(newReview);
                    db.SaveChanges();
                }

                SelectedBook.ReviewComment = ReviewText!;
                LoadBooks(); // обновляем список для корректного отображения

                SelectedBook = null;
                SelectedRating = null;
                ReviewText = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления книги: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadBooks()
        {
            using (var db = new AppDbContext())
            {
                var books = db.Books
                    .AsNoTracking()
                    .MapBookToDto()
                    .ToList();
                if(books != null)
                {
                    BookListDtos = new ObservableCollection<BookListDto>(books);
                }
            }
        }

        private void SortBooks()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    switch (SelectedSortOption)
                    {
                        case "Наивысший рейтинг":

                            var books = db.Books
                                          .MapBookToDto()
                                          .OrderBooksBy(OrderByOptions.ByVotes)
                                          .ToList();

                            if (books != null)
                                BookListDtos = new ObservableCollection<BookListDto>(books);

                            return;

                        case "Сначала новые":

                            var books1 = db.Books
                                          .MapBookToDto()
                                          .OrderBooksBy(OrderByOptions.ByPublicationDate)
                                          .ToList();

                            if (books1 != null)
                                BookListDtos = new ObservableCollection<BookListDto>(books1);

                            return;

                        case "Сначала дешевые":

                            var books2 = db.Books
                                          .MapBookToDto()
                                          .OrderBooksBy(OrderByOptions.ByPriceLowestFirst)
                                          .ToList();

                            if (books2 != null)
                                BookListDtos = new ObservableCollection<BookListDto>(books2);

                            return;

                        case "Сначала дорогие":

                            var books3 = db.Books
                                           .MapBookToDto()
                                           .OrderBooksBy(OrderByOptions.ByPriceHighestFirst)
                                           .ToList();

                            if(books3 != null)
                                BookListDtos = new ObservableCollection<BookListDto> (books3);

                            return;

                        case "По названию":
                            var books4 = db.Books
                                           .MapBookToDto()
                                           .OrderBooksBy(OrderByOptions.ByTitle)
                                           .ToList();

                            if (books4 != null)
                                BookListDtos = new ObservableCollection<BookListDto>(books4);

                            return;

                        default:
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при попытке отсортировать список: {ex.Message}\n{ex.InnerException?.Message}", 
                    "Ошибка сортировки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Вынос валидации в отдельный метод для читаемости (при заполнении книги для добавления в БД)
        private int BaseValidation(out string errorsString)
        {
            var errors = new List<string>();

            // Первостепенная проверка на заполненность всех полей
            if (string.IsNullOrWhiteSpace(Title))
                errors.Add("Название книги обязательно");
            if (string.IsNullOrWhiteSpace(Description))
                errors.Add("Описание книги обязательно");
            if (string.IsNullOrWhiteSpace(PublishedOn))
                errors.Add("Дата публикации книги обязательна");
            if (string.IsNullOrWhiteSpace(Publisher))
                errors.Add("Издатель книги обязателен");
            if (string.IsNullOrWhiteSpace(Price))
                errors.Add("Цена книги обязательна");
            if (string.IsNullOrWhiteSpace(AuthorName))
                errors.Add("Имя автора книги обязательно");
            if (string.IsNullOrWhiteSpace(Tag))
                errors.Add("Тег книги обязателен");

            // Второстепенная проверка на валидацию
            if (!DateTime.TryParse(PublishedOn, out _))
                errors.Add("Неверный формат даты (Пример: 2023-01-15)");
            if (!decimal.TryParse(Price, out _))
                errors.Add("Неверный формат цены (Пример: 19,99)");

            // Объединение всех возможных ошибок в одну строку для вывода в MessageBox для пользователя
            errorsString = string.Join("\n", errors);

            return errors.Count;
        }
    }
}
