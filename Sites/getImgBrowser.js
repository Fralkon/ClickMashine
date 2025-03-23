var js = {0}.getBoundingClientRect().toJSON();
JSON.stringify({ X: parseInt(js.x), Y: parseInt(js.y),  Height: parseInt(js.height), Width: parseInt(js.width)});