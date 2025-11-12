// Hover cards efeito extra
const cards = document.querySelectorAll('.card');
cards.forEach(card => {
    card.addEventListener('mouseenter', () => card.classList.add('shadow-xl'));
    card.addEventListener('mouseleave', () => card.classList.remove('shadow-xl'));
});
