// Tüm sayfalarda telefon input'ları için otomatik mask ekleme
document.addEventListener('DOMContentLoaded', function () {
    // Telefon input'larını bul
    const phoneInputs = document.querySelectorAll('input[type="tel"], input[name*="Phone"]');

    phoneInputs.forEach(function (input) {
        // Placeholder ekle
        if (!input.placeholder) {
            input.placeholder = '+90 xxx-xxx-xx-xx';
        }

        // Input event listener - otomatik formatla
        input.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\D/g, ''); // Sadece rakamlar

            // 90 ile başlamıyorsa ekle
            if (!value.startsWith('90')) {
                if (value.length > 0) {
                    value = '90' + value;
                }
            }

            // Format uygula: +90 xxx-xxx-xx-xx
            let formatted = '';
            if (value.length > 0) {
                formatted = '+90';
                if (value.length > 2) formatted += ' ' + value.substring(2, 5);
                if (value.length > 5) formatted += '-' + value.substring(5, 8);
                if (value.length > 8) formatted += '-' + value.substring(8, 10);
                if (value.length > 10) formatted += '-' + value.substring(10, 12);
            }

            e.target.value = formatted;
        });

        // Focus olduğunda +90 ekle
        input.addEventListener('focus', function (e) {
            if (e.target.value === '') {
                e.target.value = '+90 ';
            }
        });

        // Backspace ile +90'ı koruma
        input.addEventListener('keydown', function (e) {
            if (e.key === 'Backspace' && e.target.value === '+90 ') {
                e.preventDefault();
            }
        });
    });
});
