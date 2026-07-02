using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Practice.Common;
using Practice.DTO;
using Practice.Model;
using Practice.Service;

namespace LibraryWinForms;

public partial class Form1 : Form
{
    // ---- injected services ----
    private readonly IBookService _bookService;
    private readonly IUserService _userService;
    private readonly IUserBooksServices _userBooksService;

    // ---- Books tab controls ----
    private TextBox _txtBookId = null!;
    private TextBox _txtBookName = null!;
    private TextBox _txtBookAuthor = null!;
    private TextBox _txtBookPrice = null!;
    private TextBox _txtSearchAuthor = null!;
    private TextBox _txtSearchMaxPrice = null!;
    private DataGridView _dgvBooks = null!;

    // ---- Users tab controls ----
    private TextBox _txtUserId = null!;
    private TextBox _txtUserName = null!;
    private ComboBox _cmbRole = null!;
    private DataGridView _dgvUsers = null!;

    // ---- User Books tab controls ----
    private TextBox _txtUbUserId = null!;
    private TextBox _txtUbBookId = null!;
    private TextBox _txtUbBookName = null!;
    private TextBox _txtUbBookAuthor = null!;
    private TextBox _txtUbBookPrice = null!;
    private TextBox _txtUbSerial = null!;
    private DataGridView _dgvUserBooks = null!;

    // =========================================================
    //  THEME
    // =========================================================
    private static readonly Color ColorBackground = Color.FromArgb(24, 24, 24);
    private static readonly Color ColorPanel = Color.FromArgb(32, 32, 32);
    private static readonly Color ColorControl = Color.FromArgb(45, 45, 48);
    private static readonly Color ColorBorder = Color.FromArgb(63, 63, 70);
    private static readonly Color ColorAccent = Color.FromArgb(0, 120, 215);
    private static readonly Color ColorAccentHover = Color.FromArgb(28, 142, 235);
    private static readonly Color ColorDanger = Color.FromArgb(196, 43, 51);
    private static readonly Color ColorDangerHover = Color.FromArgb(216, 63, 71);
    private static readonly Color ColorSecondary = Color.FromArgb(63, 63, 70);
    private static readonly Color ColorSecondaryHover = Color.FromArgb(82, 82, 90);
    private static readonly Color ColorTextPrimary = Color.FromArgb(240, 240, 240);
    private static readonly Color ColorTextSecondary = Color.FromArgb(170, 170, 175);
    private static readonly Color ColorGridAltRow = Color.FromArgb(40, 40, 40);

    private static readonly Font FontHeader = new("Segoe UI", 12f, FontStyle.Bold);
    private static readonly Font FontBody = new("Segoe UI", 10f);
    private static readonly Font FontBodyBold = new("Segoe UI", 10f, FontStyle.Bold);

    private enum ButtonKind { Primary, Danger, Secondary }

    public Form1(IBookService bookService, IUserService userService, IUserBooksServices userBooksService)
    {
        _bookService = bookService;
        _userService = userService;
        _userBooksService = userBooksService;

        BuildUi();

        // Load initial data once the form is shown.
        Shown += (_, _) =>
        {
            RefreshBooksGrid();
            RefreshUsersGrid();
        };
    }

    // =========================================================
    //  UI CONSTRUCTION (code-only, no designer file)
    // =========================================================
    private void BuildUi()
    {
        Text = "Library Manager";
        Width = 1080;
        Height = 700;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(860, 540);
        BackColor = ColorBackground;
        ForeColor = ColorTextPrimary;
        Font = FontBody;

        var tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            DrawMode = TabDrawMode.OwnerDrawFixed,
            ItemSize = new Size(160, 40),
            SizeMode = TabSizeMode.Fixed,
            Padding = new Point(16, 8),
            Font = FontBodyBold,
            Appearance = TabAppearance.Normal
        };
        tabControl.DrawItem += TabControl_DrawItem;

        var tabBooks = new TabPage("Books") { BackColor = ColorPanel, Padding = new Padding(0) };
        var tabUsers = new TabPage("Users") { BackColor = ColorPanel, Padding = new Padding(0) };
        var tabUserBooks = new TabPage("User Books") { BackColor = ColorPanel, Padding = new Padding(0) };

        BuildBooksTab(tabBooks);
        BuildUsersTab(tabUsers);
        BuildUserBooksTab(tabUserBooks);

        tabControl.TabPages.Add(tabBooks);
        tabControl.TabPages.Add(tabUsers);
        tabControl.TabPages.Add(tabUserBooks);

        // Outer padding so the dark background frames the whole tab strip
        var outer = new Panel { Dock = DockStyle.Fill, BackColor = ColorBackground, Padding = new Padding(12) };
        outer.Controls.Add(tabControl);

        Controls.Add(outer);
    }

    /// <summary>Custom flat, dark-themed tab header rendering.</summary>
    private void TabControl_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (sender is not TabControl tabControl) return;

        var tabPage = tabControl.TabPages[e.Index];
        var tabBounds = tabControl.GetTabRect(e.Index);
        bool selected = e.Index == tabControl.SelectedIndex;

        using var backBrush = new SolidBrush(selected ? ColorPanel : ColorBackground);
        e.Graphics.FillRectangle(backBrush, tabBounds);

        if (selected)
        {
            using var accentBrush = new SolidBrush(ColorAccent);
            e.Graphics.FillRectangle(accentBrush, tabBounds.X, tabBounds.Bottom - 3, tabBounds.Width, 3);
        }

        TextRenderer.DrawText(
            e.Graphics,
            tabPage.Text,
            FontBodyBold,
            tabBounds,
            selected ? ColorTextPrimary : ColorTextSecondary,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    // ---------------------------------------------------------
    //  BOOKS TAB
    // ---------------------------------------------------------
    private void BuildBooksTab(TabPage page)
    {
        var inputPanel = CreateInputPanel();

        _txtBookId = AddLabeledTextBox(inputPanel, "Book ID:", 60);
        _txtBookName = AddLabeledTextBox(inputPanel, "Name:", 130);
        _txtBookAuthor = AddLabeledTextBox(inputPanel, "Author:", 130);
        _txtBookPrice = AddLabeledTextBox(inputPanel, "Price:", 80);

        var btnAdd = CreateButton("Add Book", ButtonKind.Primary);
        btnAdd.Click += BtnAddBook_Click;

        var btnDelete = CreateButton("Delete (by ID)", ButtonKind.Danger);
        btnDelete.Click += BtnDeleteBook_Click;

        var btnBorrow = CreateButton("Borrow (by ID)", ButtonKind.Primary);
        btnBorrow.Click += BtnBorrowBook_Click;

        var btnReturn = CreateButton("Return (by ID)", ButtonKind.Secondary);
        btnReturn.Click += BtnReturnBook_Click;

        var btnRefresh = CreateButton("Refresh", ButtonKind.Secondary);
        btnRefresh.Click += (_, _) => RefreshBooksGrid();

        inputPanel.Controls.Add(btnAdd);
        inputPanel.Controls.Add(btnDelete);
        inputPanel.Controls.Add(btnBorrow);
        inputPanel.Controls.Add(btnReturn);
        inputPanel.Controls.Add(btnRefresh);

        var searchPanel = CreateInputPanel();
        _txtSearchAuthor = AddLabeledTextBox(searchPanel, "Search Author:", 130);
        _txtSearchMaxPrice = AddLabeledTextBox(searchPanel, "Max Price:", 80);

        var btnSearch = CreateButton("Search", ButtonKind.Primary);
        btnSearch.Click += BtnSearchBooks_Click;
        searchPanel.Controls.Add(btnSearch);

        _dgvBooks = CreateGrid();

        page.Controls.Add(_dgvBooks);
        page.Controls.Add(CreateSeparator());
        page.Controls.Add(searchPanel);
        page.Controls.Add(CreateSeparator());
        page.Controls.Add(inputPanel);
    }

    private void BtnAddBook_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtBookName.Text) || string.IsNullOrWhiteSpace(_txtBookAuthor.Text))
        {
            ShowWarning("Name and Author are required.");
            return;
        }

        if (!decimal.TryParse(_txtBookPrice.Text, out var price) || price < 0)
        {
            ShowWarning("Please enter a valid, non-negative price.");
            return;
        }

        try
        {
            // serialNumber is generated by BookService, so we just pass a placeholder.
            var dto = new CreateBookDto(_txtBookName.Text.Trim(), _txtBookAuthor.Text.Trim(), price, string.Empty);
            var created = _bookService.CreateBook(dto);
            ShowInfo($"Book '{created.Name}' created with ID {created.Id} and serial {created.SerialNumber}.");
            ClearBookInputs();
            RefreshBooksGrid();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void BtnDeleteBook_Click(object? sender, EventArgs e)
    {
        if (!TryGetId(_txtBookId.Text, out var id))
            return;

        try
        {
            var deleted = _bookService.DeleteBook(id);
            if (!deleted)
                ShowWarning($"No book found with ID {id}.");
            else
                ShowInfo($"Book {id} deleted.");

            RefreshBooksGrid();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void BtnBorrowBook_Click(object? sender, EventArgs e)
    {
        if (!TryGetId(_txtBookId.Text, out var id))
            return;

        try
        {
            Result<Book> result = _bookService.BorrowBook(_bookService, id);
            HandleResult(result, book => $"'{book.Name}' borrowed successfully.");
            RefreshBooksGrid();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void BtnReturnBook_Click(object? sender, EventArgs e)
    {
        if (!TryGetId(_txtBookId.Text, out var id))
            return;

        try
        {
            Result<Book> result = _bookService.returnBook(_bookService, id);
            HandleResult(result, book => $"'{book.Name}' returned successfully.");
            RefreshBooksGrid();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void BtnSearchBooks_Click(object? sender, EventArgs e)
    {
        decimal? maxPrice = null;
        if (!string.IsNullOrWhiteSpace(_txtSearchMaxPrice.Text))
        {
            if (!decimal.TryParse(_txtSearchMaxPrice.Text, out var parsed))
            {
                ShowWarning("Max price must be a valid number.");
                return;
            }
            maxPrice = parsed;
        }

        var author = string.IsNullOrWhiteSpace(_txtSearchAuthor.Text) ? null : _txtSearchAuthor.Text.Trim();

        try
        {
            Result<List<Book>> result = _bookService.SearchBooks(_bookService, author, maxPrice);
            if (!result.isSuccess)
            {
                ShowWarning(result.ErrorMessage ?? "No books matched your search.");
                _dgvBooks.DataSource = new List<Book>();
                return;
            }

            _dgvBooks.DataSource = null;
            _dgvBooks.DataSource = result.Data;
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void RefreshBooksGrid()
    {
        try
        {
            var books = _bookService.GetAllBooks();
            _dgvBooks.DataSource = null;
            _dgvBooks.DataSource = books;
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void ClearBookInputs()
    {
        _txtBookName.Clear();
        _txtBookAuthor.Clear();
        _txtBookPrice.Clear();
    }

    // ---------------------------------------------------------
    //  USERS TAB
    // ---------------------------------------------------------
    private void BuildUsersTab(TabPage page)
    {
        var inputPanel = CreateInputPanel();

        _txtUserId = AddLabeledTextBox(inputPanel, "User ID:", 60);
        _txtUserName = AddLabeledTextBox(inputPanel, "Name:", 150);

        var roleLabel = CreateLabel("Role:");
        _cmbRole = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat,
            Width = 100,
            Font = FontBody,
            BackColor = ColorControl,
            ForeColor = ColorTextPrimary,
            Margin = new Padding(0, 8, 15, 8)
        };
        _cmbRole.Items.AddRange(Enum.GetNames(typeof(Role)));
        _cmbRole.SelectedIndex = 0;

        inputPanel.Controls.Add(roleLabel);
        inputPanel.Controls.Add(_cmbRole);

        var btnAdd = CreateButton("Add User", ButtonKind.Primary);
        btnAdd.Click += BtnAddUser_Click;

        var btnDelete = CreateButton("Delete (by ID)", ButtonKind.Danger);
        btnDelete.Click += BtnDeleteUser_Click;

        var btnUpdateRole = CreateButton("Update Role (by ID)", ButtonKind.Primary);
        btnUpdateRole.Click += BtnUpdateUserRole_Click;

        var btnRefresh = CreateButton("Refresh", ButtonKind.Secondary);
        btnRefresh.Click += (_, _) => RefreshUsersGrid();

        inputPanel.Controls.Add(btnAdd);
        inputPanel.Controls.Add(btnDelete);
        inputPanel.Controls.Add(btnUpdateRole);
        inputPanel.Controls.Add(btnRefresh);

        _dgvUsers = CreateGrid();

        page.Controls.Add(_dgvUsers);
        page.Controls.Add(CreateSeparator());
        page.Controls.Add(inputPanel);
    }

    private void BtnAddUser_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtUserName.Text))
        {
            ShowWarning("Name is required.");
            return;
        }

        try
        {
            var dto = new CreateUserDto(_txtUserName.Text.Trim());
            var created = _userService.AddUser(dto);
            ShowInfo($"User '{created.Name}' created with ID {created.id}.");
            _txtUserName.Clear();
            RefreshUsersGrid();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void BtnDeleteUser_Click(object? sender, EventArgs e)
    {
        if (!TryGetId(_txtUserId.Text, out var id))
            return;

        try
        {
            var deleted = _userService.RemoveUser(id);
            if (deleted is null)
                ShowWarning($"No user found with ID {id}.");
            else
                ShowInfo($"User '{deleted.Name}' deleted.");

            RefreshUsersGrid();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void BtnUpdateUserRole_Click(object? sender, EventArgs e)
    {
        if (!TryGetId(_txtUserId.Text, out var id))
            return;

        if (_cmbRole.SelectedItem is null || !Enum.TryParse<Role>((string)_cmbRole.SelectedItem, out var role))
        {
            ShowWarning("Please select a role.");
            return;
        }

        try
        {
            var updated = _userService.UpdateUser(id, new UpdateUserRoleDto(role));
            if (updated is null)
                ShowWarning($"No user found with ID {id}.");
            else
                ShowInfo($"User '{updated.Name}' role updated to {role}.");

            RefreshUsersGrid();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void RefreshUsersGrid()
    {
        try
        {
            var users = _userService.GetUsers();
            _dgvUsers.DataSource = null;
            _dgvUsers.DataSource = users;
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    // ---------------------------------------------------------
    //  USER BOOKS TAB
    // ---------------------------------------------------------
    private void BuildUserBooksTab(TabPage page)
    {
        var lookupPanel = CreateInputPanel();

        _txtUbUserId = AddLabeledTextBox(lookupPanel, "User ID:", 60);

        var btnGet = CreateButton("Get User's Books", ButtonKind.Secondary);
        btnGet.Click += BtnGetUserBooks_Click;
        lookupPanel.Controls.Add(btnGet);

        var addPanel = CreateInputPanel();

        _txtUbBookId = AddLabeledTextBox(addPanel, "Book ID:", 60);
        _txtUbBookName = AddLabeledTextBox(addPanel, "Name:", 120);
        _txtUbBookAuthor = AddLabeledTextBox(addPanel, "Author:", 120);
        _txtUbBookPrice = AddLabeledTextBox(addPanel, "Price:", 70);
        _txtUbSerial = AddLabeledTextBox(addPanel, "Serial:", 90);

        var btnAddBook = CreateButton("Add Book to User", ButtonKind.Primary);
        btnAddBook.Click += BtnAddBookToUser_Click;

        var btnRemoveBook = CreateButton("Remove Book (by Book ID)", ButtonKind.Danger);
        btnRemoveBook.Click += BtnRemoveBookFromUser_Click;

        addPanel.Controls.Add(btnAddBook);
        addPanel.Controls.Add(btnRemoveBook);

        _dgvUserBooks = CreateGrid();

        page.Controls.Add(_dgvUserBooks);
        page.Controls.Add(CreateSeparator());
        page.Controls.Add(addPanel);
        page.Controls.Add(CreateSeparator());
        page.Controls.Add(lookupPanel);
    }

    private void BtnGetUserBooks_Click(object? sender, EventArgs e)
    {
        if (!TryGetId(_txtUbUserId.Text, out var id))
            return;

        try
        {
            UserBooksDto result = _userBooksService.GetUserBooks(id);
            if (result is null)
            {
                ShowWarning($"No user found with ID {id}.");
                _dgvUserBooks.DataSource = new List<Book>();
                return;
            }

            Text = $"Library Manager — viewing books for '{result.Name}' (ID {id})";
            _dgvUserBooks.DataSource = null;
            _dgvUserBooks.DataSource = result.books;
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void BtnAddBookToUser_Click(object? sender, EventArgs e)
    {
        if (!TryGetId(_txtUbUserId.Text, out var userId))
            return;

        if (string.IsNullOrWhiteSpace(_txtUbBookName.Text) || string.IsNullOrWhiteSpace(_txtUbBookAuthor.Text))
        {
            ShowWarning("Name and Author are required.");
            return;
        }

        if (!decimal.TryParse(_txtUbBookPrice.Text, out var price) || price < 0)
        {
            ShowWarning("Please enter a valid, non-negative price.");
            return;
        }

        try
        {
            var dto = new CreateBookDto(
                _txtUbBookName.Text.Trim(),
                _txtUbBookAuthor.Text.Trim(),
                price,
                string.IsNullOrWhiteSpace(_txtUbSerial.Text) ? string.Empty : _txtUbSerial.Text.Trim());

            var result = _userBooksService.CreateUserBooks(userId, dto);
            if (result is null)
            {
                ShowWarning($"No user found with ID {userId}.");
                return;
            }

            ShowInfo($"'{result.Name}' added to user {userId}.");
            BtnGetUserBooks_Click(sender, e); // refresh the grid for that user
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void BtnRemoveBookFromUser_Click(object? sender, EventArgs e)
    {
        if (!TryGetId(_txtUbUserId.Text, out var userId))
            return;

        if (!TryGetId(_txtUbBookId.Text, out var bookId))
            return;

        try
        {
            var removed = _userBooksService.DeleteUserBooks(userId, bookId);
            if (removed is null)
                ShowWarning("No matching user/book combination was found.");
            else
                ShowInfo($"'{removed.Name}' removed from user {userId}.");

            BtnGetUserBooks_Click(sender, e); // refresh the grid for that user
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    // =========================================================
    //  SHARED UI HELPERS (styling lives here)
    // =========================================================
    private static FlowLayoutPanel CreateInputPanel() => new()
    {
        Dock = DockStyle.Top,
        AutoSize = true,
        FlowDirection = FlowDirection.LeftToRight,
        BackColor = ColorPanel,
        Padding = new Padding(16, 12, 16, 12),
        WrapContents = true
    };

    /// <summary>A thin 1px divider that separates the input areas from the grid.</summary>
    private static Panel CreateSeparator() => new()
    {
        Dock = DockStyle.Top,
        Height = 1,
        BackColor = ColorBorder
    };

    private static Label CreateLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Font = FontBody,
        ForeColor = ColorTextSecondary,
        BackColor = Color.Transparent,
        TextAlign = ContentAlignment.MiddleLeft,
        Padding = new Padding(0, 10, 6, 0),
        Margin = new Padding(0, 5, 0, 5)
    };

    private static TextBox AddLabeledTextBox(FlowLayoutPanel parent, string labelText, int width)
    {
        var label = CreateLabel(labelText);

        var textBox = new TextBox
        {
            Width = width,
            Font = FontBody,
            BackColor = ColorControl,
            ForeColor = ColorTextPrimary,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 6, 20, 6)
        };

        parent.Controls.Add(label);
        parent.Controls.Add(textBox);
        return textBox;
    }

    private static Button CreateButton(string text, ButtonKind kind = ButtonKind.Primary)
    {
        var (back, hover) = kind switch
        {
            ButtonKind.Danger => (ColorDanger, ColorDangerHover),
            ButtonKind.Secondary => (ColorSecondary, ColorSecondaryHover),
            _ => (ColorAccent, ColorAccentHover)
        };

        var button = new Button
        {
            Text = text,
            Font = FontBodyBold,
            FlatStyle = FlatStyle.Flat,
            BackColor = back,
            ForeColor = Color.White,
            UseVisualStyleBackColor = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(16, 8, 16, 8),
            Margin = new Padding(0, 6, 10, 6),
            Cursor = Cursors.Hand
        };

        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = hover;
        button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(back, 0.15f);

        return button;
    }

    private static DataGridView CreateGrid()
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            BorderStyle = BorderStyle.None,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            ColumnHeadersHeight = 42,
            RowTemplate = { Height = 34 },
            BackgroundColor = ColorPanel,
            GridColor = ColorBorder,
            EnableHeadersVisualStyles = false,
            Font = FontBody
        };

        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = ColorControl,
            ForeColor = ColorTextPrimary,
            SelectionBackColor = ColorControl,
            SelectionForeColor = ColorTextPrimary,
            Font = FontBodyBold,
            Alignment = DataGridViewContentAlignment.MiddleLeft,
            Padding = new Padding(10, 0, 0, 0)
        };

        grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = ColorPanel,
            ForeColor = ColorTextPrimary,
            SelectionBackColor = ColorAccent,
            SelectionForeColor = Color.White,
            Padding = new Padding(10, 4, 4, 4),
            Font = FontBody
        };

        grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = ColorGridAltRow,
            ForeColor = ColorTextPrimary,
            SelectionBackColor = ColorAccent,
            SelectionForeColor = Color.White
        };

        return grid;
    }

    private static bool TryGetId(string text, out int id)
    {
        if (int.TryParse(text, out id) && id > 0)
            return true;

        ShowWarning("Please enter a valid, positive ID.");
        id = 0;
        return false;
    }

    private void HandleResult<T>(Result<T> result, Func<T, string> successMessage)
    {
        if (result.isSuccess && result.Data is not null)
            ShowInfo(successMessage(result.Data));
        else
            ShowWarning(result.ErrorMessage ?? "The operation failed.");
    }

    private static void ShowInfo(string message) =>
        MessageBox.Show(message, "Library Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);

    private static void ShowWarning(string message) =>
        MessageBox.Show(message, "Library Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private static void ShowError(Exception ex) =>
        MessageBox.Show(ex.Message, "Unexpected Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
}