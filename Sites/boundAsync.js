var target = {0};
const config = {
    childList: true,
    attributes: true,
    subtree: true,
    characterData: true,
    attributeOldValue: true,
    characterDataOldValue: true
};
const callback = function (mutationsList, observer) {
    (async function () {
        await CefSharp.BindObjectAsync("boundAsync");
        boundAsync.event(target.innerText);
    })();
};
const observer = new MutationObserver(callback);
observer.observe(target, config);