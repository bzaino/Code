define([
    'jquery',
    'dust',
    'salt'
], function ($, dust, SALT) {
    var membershipId;
    $('.occupations').hide();
    $('.results').hide();
    $('.reset').hide();
    $('.subtitleRes').hide();
    if ($('#membershipId').text() !== '') {
        membershipId = $('#membershipId').text();
    } else {
        membershipId = 0;
    }
    $('.next').attr('disabled', true);

    function go() {
        stateDropdown.disabled = true;
        schoolDropdown.disabled = true;
        if (majorDropdown.selectedIndex > 0) {
            stateDropdown.disabled = false;
        } else {
            stateDropdown.selectedIndex = 0;
            schoolDropdown.selectedIndex = 0;
            $('.next').attr('disabled', true);
        }
        if (stateDropdown.selectedIndex > 0 && majorDropdown.selectedIndex > 0) {
            schoolDropdown.disabled = false;
            majorSelected = parseInt(majorDropdown.value, 10);
            stateSelected = parseInt(stateDropdown.value, 10);
            $.ajax({
                url: '/api/surveyservice/JSI/Schools/' + majorSelected + '/' + stateSelected
            })
                .done(function (data) {

                data.JSIQuizList.sort(function (a, b) {
                    return a.School.localeCompare(b.School);
                });

                dust.render('JSISchoolDropDown', data, function (err, out) {
                    $('#sel3').html(out);
                });
                $('.next').attr('disabled', true);
            });
        } else {
            schoolDropdown.selectedIndex = 0;
            $('.next').attr('disabled', true);
        }
    }

    function reset() {
        $('.question').show();
        $('.results').hide();
        $('.reset').hide();
        $('.next').show();
        $('.longText').show();
        $('.subtitleStart').show();
        $('.subtitleRes').hide();
        $('.occupations').hide();
        $('.js-jsiDropdown').show();
    }

    function next() {
        $('.question').hide();
        $('.results').show();
        $('.reset').show();
        $('.next').hide();
        $('.subtitleRes').show();
        $('.subtitleStart').hide();
        $('.longText').hide();
        $('.occupations').show();
        $('.js-jsiDropdown').hide();
    }
    var majorDropdown = document.getElementById('sel1');
    var stateDropdown = document.getElementById('sel2');
    var schoolDropdown = document.getElementById('sel3');
    var majorSelected = '',
        stateSelected = '';
    $('#sel1').change(function (e) {
        go();
    });
    $('#sel2').change(function (e) {
        go();
    });
    $('#sel3').change(function (e) {
        if (schoolDropdown.selectedIndex > 0) {
            var schoolSelected = schoolDropdown.options[schoolDropdown.selectedIndex].text;
            $('.next').attr('disabled', false);
            total = 0;
            $.ajax({
                accepts: 'application/json',
                url: '/api/surveyservice/JSI/',
                type: 'PUT',
                contentType: 'application/json',
                data: JSON.stringify({
                    SchoolId: schoolDropdown.value,
                    MajorId: majorSelected,
                    MemberId: membershipId
                })
            }).done(function (data) {
                data.MajorText = majorDropdown.options[majorDropdown.selectedIndex].text;
                data.StateText = stateDropdown.options[stateDropdown.selectedIndex].text;
                data.SchoolText = schoolSelected;
                dust.render('SalaryEstimatorResults', data, function (err, out) {
                    $('.occupations').html(out);
                    $('.average').each(function (i, obj) {
                        var average = parseInt(obj.innerText, 10);
                        if (isNaN(average)) {
                            average = parseInt(obj.textContent, 10);
                        }
                        total += average;
                        iter = i + 1;
                        averageComs = numberWithCommas(Math.floor(average));
                        obj.innerText = averageComs;
                        obj.textContent = averageComs;
                    });
                    finalAverage = total / iter;
                    finalAverage = numberWithCommas(Math.floor(finalAverage));
                    $('.finalAverage').append(finalAverage);
                });
            });
        } else {
            $('.next').attr('disabled', true);
        }
    });
    $('.next').click(function (e) {
        next();
    });
    $('.reset').click(function (e) {
        reset();
    });

    function numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    }

    $('#js-salaryestimator-get-results').click(function () {
        SALT.publish('content:todo:completed', {
            contentId: $('.js-todoContainer').attr('data-primary-key'),
            contentType: $('.js-todoContainer').attr('data-content-type'),
            contentTitle: $('.js-todoContainer').attr('data-content-title')
        });
    });
});
