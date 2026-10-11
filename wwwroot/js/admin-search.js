// File: wwwroot/js/admin-search.js

// Bọc code trong 1 hàm để tránh xung đột biến
(function () {
    const searchInput = document.getElementById('admin-search-input');
    const suggestionsContainer = document.getElementById('admin-search-suggestions');

    // Nếu không tìm thấy ô search (ví dụ: trang login) thì không làm gì cả
    if (!searchInput || !suggestionsContainer) {
        return;
    }

    // --- 1. Hàm Debounce ---
    // Hàm này dùng để trì hoãn việc gọi API
    // Chúng ta chỉ gọi API sau khi người dùng ngừng gõ 300ms
    let debounceTimer;
    function debounce(func, delay) {
        return function () {
            const context = this;
            const args = arguments;
            clearTimeout(debounceTimer);
            debounceTimer = setTimeout(() => func.apply(context, args), delay);
        };
    }

    // --- 2. Hàm gọi API để lấy gợi ý ---
    async function fetchSuggestions(term) {
        // Chỉ tìm khi có ít nhất 2 ký tự
        if (term.length < 2) {
            suggestionsContainer.innerHTML = '';
            suggestionsContainer.style.display = 'none';
            return;
        }

        try {
            // URL gọi đến handler OnGetSuggestionsAsync trong SearchModel.cs
            const response = await fetch(`/Admin/Search?handler=Suggestions&term=${encodeURIComponent(term)}`);

            if (!response.ok) {
                throw new Error('Lỗi mạng');
            }

            const suggestions = await response.json();
            renderSuggestions(suggestions, term);
        } catch (error) {
            console.error('Lỗi khi lấy gợi ý:', error);
            suggestionsContainer.innerHTML = '';
            suggestionsContainer.style.display = 'none';
        }
    }

    // --- 3. Hàm hiển thị gợi ý ra HTML ---
    function renderSuggestions(suggestions, term) {
        if (suggestions.length === 0) {
            // Hiển thị "Không tìm thấy" nếu không có kết quả
            suggestionsContainer.innerHTML = '<span class="list-group-item text-muted">Không tìm thấy kết quả nào.</span>';
            suggestionsContainer.style.display = 'block';
            return;
        }

        // Hàm này để tô đậm từ khóa tìm kiếm
        function getHighlightedText(text, highlight) {
            // Sử dụng 'gi' (global, insensitive) để tìm và thay thế tất cả
            const parts = text.split(new RegExp(`(${highlight})`, 'gi'));
            return parts.map(part =>
                part.toLowerCase() === highlight.toLowerCase()
                    ? `<strong>${part}</strong>`
                    : part
            ).join('');
        }

        let currentCategory = '';
        suggestionsContainer.innerHTML = suggestions.map(s => {
            let categoryHeader = '';
            // Thêm tiêu đề (ví dụ "Môn học") khi đổi sang loại mới
            if (s.category !== currentCategory) {
                currentCategory = s.category;
                categoryHeader = `<h6 class="dropdown-header text-muted small ps-3 pt-2 pb-0">${s.category}</h6>`;
            }

            // Dùng @Html.Raw bên C# để tránh lỗi XSS, nhưng ở đây ta highlight
            // nên tạm chấp nhận, vì `s.label` là từ DB
            const highlightedLabel = getHighlightedText(s.label, term);

            return `${categoryHeader}
                    <a href="${s.url}" class="list-group-item list-group-item-action">
                        ${highlightedLabel}
                    </a>`;
        }).join('');

        suggestionsContainer.style.display = 'block';
    }

    // --- 4. Gắn Event Listener vào ô input ---
    searchInput.addEventListener('input', debounce(function () {
        fetchSuggestions(this.value);
    }, 300)); // Chờ 300ms

    // --- 5. Ẩn gợi ý khi bấm ra ngoài ---
    document.addEventListener('click', function (e) {
        // Nếu click vào bên ngoài .search-bar-wrapper
        if (!e.target.closest('.search-bar-wrapper')) {
            suggestionsContainer.innerHTML = '';
            suggestionsContainer.style.display = 'none';
        }
    });

})();