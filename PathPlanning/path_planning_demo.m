function path_planning_demo()
    % %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    % %%      PATH PLANNING VISUALIZATION DEMO
    % %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    %
    % This script creates a plot visualizing a simple
    % path-planning scenario.
    %

    % --- 1. Define Key Points ---
    
    % Static Start and End points
    start_point = [5, 5];
    end_point   = [45, 40];

    % Input obstacle points here
    obstacle1 = [10, 15];
    obstacle2 = [30, 25];
    
    % (Optional) Add more obstacles
    % obstacle3 = [20, 30];

    % Combine all obstacles into a single matrix for easy plotting
    all_obstacles = [obstacle1; obstacle2];
    
    % --- 2. Define the "Avoidance Path" ---
    % This is a list of waypoints to manually dodge
    % the obstacles.
    % Path: Start -> Waypoint 1 -> Waypoint 2 -> End
    
    waypoint1 = [15, 10]; % Dodge under obstacle 1
    waypoint2 = [25, 20]; % Go between obstacles
    waypoint3 = [35, 30]; % Dodge under obstacle 2
    
    % Create the X and Y coordinates for the avoidance path
    avoidance_path_x = [start_point(1), waypoint1(1), waypoint2(1), waypoint3(1), end_point(1)];
    avoidance_path_y = [start_point(2), waypoint1(2), waypoint2(2), waypoint3(2), end_point(2)];

    % Create X and Y for the "Original" (blocked) path
    original_path_x = [start_point(1), end_point(1)];
    original_path_y = [start_point(2), end_point(2)];

    % --- 3. Create the Plot Window ---
    
    % Create a new figure window
    figure;
    hold on; % Keep 'hold on' to draw multiple lines on one plot
    
    % Set up the plot appearance
    title('Path Planning with Obstacle Avoidance');
    xlabel('X Coordinate');
    ylabel('Y Coordinate');
    grid on;
    axis equal; % Ensures X and Y scales are the same
    set(gca, 'FontSize', 12);

    % --- 4. Draw Everything on the Plot ---
    
    % Plot the Start, End, and Obstacle points
    % (Matches the legend style from the PDF [cite: 134, 161-163])
    plot(start_point(1), start_point(2), ...
        'go', 'MarkerSize', 12, 'MarkerFaceColor', 'g', 'LineWidth', 2, 'DisplayName', 'Start Point');
        
    plot(end_point(1), end_point(2), ...
        'rx', 'MarkerSize', 12, 'LineWidth', 2, 'DisplayName', 'End Point');
        
    plot(all_obstacles(:,1), all_obstacles(:,2), ...
        'ks', 'MarkerSize', 10, 'MarkerFaceColor', 'r', 'LineWidth', 2, 'DisplayName', 'Obstacles');

    % Plot the "Original" blocked path (like in the PDF )
    plot(original_path_x, original_path_y, ...
        'b--', 'LineWidth', 1.5, 'DisplayName', 'Original Path (Blocked)');

    % Plot the "Obstacle Avoidance Path" (like in the PDF [cite: 162])
    plot(avoidance_path_x, avoidance_path_y, ...
        'g-', 'LineWidth', 2.5, 'DisplayName', 'Obstacle Avoidance Path');
    
    % --- 5. Finalize Plot ---
    legend('Location', 'northwest');
    hold off; % Done plotting

end