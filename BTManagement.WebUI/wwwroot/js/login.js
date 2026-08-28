/* =========================================================
   DSİ BT YÖNETİMİ
   LOGIN JAVASCRIPT
   ========================================================= */

document.addEventListener("DOMContentLoaded", function () {

    /* =====================================================
       ŞİFRE GÖSTER / GİZLE
       ===================================================== */

    const passwordButtons = document.querySelectorAll(".password-toggle");

    passwordButtons.forEach(function (button) {

        button.addEventListener("click", function (e) {

            e.preventDefault();

            const wrapper = button.closest(".input-wrapper");

            if (!wrapper) {
                return;
            }

            const input = wrapper.querySelector(".password-input");
            const icon = button.querySelector("i");

            if (!input) {
                return;
            }

            if (input.type === "password") {

                input.type = "text";

                if (icon) {
                    icon.classList.remove("fa-eye");
                    icon.classList.add("fa-eye-slash");
                }

                button.setAttribute(
                    "title",
                    "Şifreyi gizle"
                );

            } else {

                input.type = "password";

                if (icon) {
                    icon.classList.remove("fa-eye-slash");
                    icon.classList.add("fa-eye");
                }

                button.setAttribute(
                    "title",
                    "Şifreyi göster"
                );
            }

        });

    });


    /* =====================================================
       LOGIN FORM LOADING
       ===================================================== */

    const loginForm = document.getElementById("loginForm");
    const loginButton = document.getElementById("loginButton");

    if (loginForm && loginButton) {

        loginForm.addEventListener("submit", function () {

            if (!loginForm.checkValidity()) {
                return;
            }

            loginButton.classList.add("loading");

            loginButton.innerHTML = `
                <span class="button-spinner"></span>
                <span>Giriş yapılıyor...</span>
            `;

        });

    }


    /* =====================================================
       ŞİFRE DEĞİŞTİRME LOADING
       ===================================================== */

    const forgetForm =
        document.getElementById("forgetPasswordForm");

    const changePasswordButton =
        document.getElementById("changePasswordButton");

    if (forgetForm && changePasswordButton) {

        forgetForm.addEventListener("submit", function () {

            if (!forgetForm.checkValidity()) {
                return;
            }

            changePasswordButton.disabled = true;

            changePasswordButton.innerHTML = `
                <span class="button-spinner"></span>
                <span>İşleniyor...</span>
            `;

        });

    }

});