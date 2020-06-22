document.addEventListener('DOMContentLoaded', function () {
    var datacellCollection = document.getElementsByClassName("cell");
    Array.prototype.forEach.call(datacellCollection, RemoveZeros);

    function RemoveZeros(currentDataCell) {
        if (currentDataCell.value == "0") {
            currentDataCell.value = "";
        }
    }
});

var userLevel = 1;

function ToggleUserControl(level) {
    userLevel = level;
    var datacellCollection = document.getElementsByClassName("cell");
    Array.prototype.forEach.call(datacellCollection, Toggle);
}

function Toggle(dataCell) {
    var tag = dataCell.getAttribute("tag");

    if (userLevel == "0") {
        dataCell.readOnly = true;
    }
    else if (userLevel == "1") {
        if (tag != "0") {
            dataCell.value = parseInt(dataCell.getAttribute("tag"));
            dataCell.readOnly = true;
        }
        if (tag == "0") {
            dataCell.readOnly = false;
        }
    }
    else {
        dataCell.readOnly = false;
    }

}

function ValidateInput(dataCell) {
    //Remove User made 0.
    if (dataCell.value == "0") {
        dataCell.value = "";
    }

    var tag = dataCell.getAttribute("tag");

    //ClientSide Sudoku validation?
    if (tag == "0") {
        var coordinate = dataCell.name;

        var val = dataCell.value;
        //console.log(name);
        var row = coordinate.slice(coordinate.indexOf("[") + 1, coordinate.indexOf("]"));
        //console.log(row);
        var column = coordinate.slice(coordinate.indexOf("]") + 2, coordinate.indexOf("]") + 3);
        //console.log(column);
        var datacellRow = new Array();
        var datacellColumn = new Array();
        var datacellBLock = new Array();
        for (var i = 0; i < 9; i++) {
            if (i != column) {
                datacellRow.unshift(document.getElementById("Cells_" + row + "__" + i + "_"));
            }
            if (i != row) {
                datacellColumn.unshift(document.getElementById("Cells_" + i + "__" + column + "_"));
            }
        }
        //TODO: Add block array and validation code.
        var startCol = Math.floor(parseInt(column) / 3) * 3;
        var startRow = Math.floor(parseInt(row) / 3) * 3;
        for (var i = startRow; i < startRow + 3; i++) {
            for (var j = startCol; j < startCol + 3; j++) {
                if (!(i == row && j == column)) {
                    datacellBLock.unshift(document.getElementById("Cells_" + i + "__" + j + "_"));
                }
            }
        }

        var datacellCollection = datacellBLock.concat(datacellColumn, datacellRow);

        var isUnique = true;

        if (val != "") {
            //Check for double numbers.
            datacellCollection.forEach(cell => {
                if (val == cell.value) {
                    isUnique = false;
                    cell.style.color = "deeppink";
                    if (val != 0) {
                        cell.style.backgroundColor = "pink";
                    }
                } else {
                    cell.style.color = "";
                    cell.style.backgroundColor = "";
                }
            });
        }
        if (!isUnique) {
            dataCell.style.color = "red";
        } else {
            dataCell.style.color = "";
        }
    }
}

function SelectionChanged(sudokuId) {
    location.href = 'Sudoku/ChangeSudoku/' + sudokuId;
}

function ResizeCells() {
    var board = document.getElementById("board");
    board.style.height = board.style.width;
}