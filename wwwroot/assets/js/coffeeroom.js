const shareButton = document.getElementById('shareButton');
if (shareButton != null && shareButton != undefined) {
    shareButton.addEventListener('click', async () => {
        try {
            if (navigator.share) {
                await navigator.share({
                    title: 'Check out this awesome post from jsm33t.com',
                    url: window.location.href,
                });
                console.log('Successfully shared');
            } else if (navigator.clipboard && navigator.clipboard.writeText) {
                await navigator.clipboard.writeText(window.location.href);
                console.log('URL copied to clipboard');
            } else {
                console.log('Sharing not supported');
            }
        } catch (error) {
            console.log('Error sharing:', error);
        }
    });
}

class StringBuilder {
    constructor() {
        this.buffer = "";
    }

    append(string) {
        this.buffer += string;
    }

    toString() {
        return this.buffer;
    }
}

function load_images_temp() {
    const elements = document.querySelectorAll('.img_placeholder');
    const isHD = localStorage.getItem('hd') === 'true';

    elements.forEach(element => {
        const originalString = element.getAttribute('src');
        let modifiedString = originalString.replace(new RegExp('_placeholder', 'g'), '');

        if (isHD) {
            modifiedString = modifiedString.replace(new RegExp('img_', 'g'), 'img_hd');
        }

        element.setAttribute('src', modifiedString);
    });
}

function toaster(title, message) {
    document.getElementById("toastBody").innerText = message.trim();
    document.getElementById("toastTitle").innerText = title.trim();
    var toastElement = document.getElementById("toastBox");
    var toast = new bootstrap.Toast(toastElement);
    toast.show();
}
