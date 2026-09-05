using Godot;
using System;

public partial class birdsc : RigidBody2D
{
    private bool _isDragging = false;
    private Vector2 _startPos;
    private float _power = 15.0f;
    private float _maxDragDistance = 100.0f; //Limite para esticar
    private float _gravity = 980.0f;

    private Line2D _trajectory;

    public override void _Ready()
    {
        _startPos = GlobalPosition;
        Freeze = true; 
        
        //Função que obtém o nó que simula visualmente a trajetória 
        _trajectory = GetNode<Line2D>("Trajectory");
        _trajectory.Hide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouse && mouse.ButtonIndex == MouseButton.Left)
        {
            if (mouse.Pressed && GlobalPosition.DistanceTo(GetGlobalMousePosition()) < 50)
            {
                _isDragging = true;
                _trajectory.Show(); //Exibe a trajetória que o pássaro irá percorrer 
            }
            else if (_isDragging && !mouse.Pressed)
            {
                Release();
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isDragging)
        {
            Vector2 mousePos = GetGlobalMousePosition();
            Vector2 pullVector = _startPos - mousePos;
            
            //Limita a distância máxima a qual o estilingue pode ser puxado
            if (pullVector.Length() > _maxDragDistance)
            {
                pullVector = pullVector.Normalized() * _maxDragDistance;
            }
            
            GlobalPosition = _startPos - pullVector;

            //Desenho da trajetória
            UpdateTrajectory(pullVector * _power);
        }
    }

    private void UpdateTrajectory(Vector2 initialVelocity)
    {
        _trajectory.ClearPoints();
        Vector2 currentPos = Vector2.Zero; //Posição inicial do pássaro
        Vector2 currentVelocity = initialVelocity;
        float timeStep = 0.1f;
        
        for (int i = 0; i < 30; i++)
        {
            _trajectory.AddPoint(currentPos);
            currentVelocity.Y += _gravity * timeStep;
            currentPos += currentVelocity * timeStep;
        }
    }

    private void Release()
    {
        _isDragging = false;
        Freeze = false;
        _trajectory.Hide(); //Esconde a mira depois de atirar
        
        Vector2 pullVector = _startPos - GlobalPosition;
        ApplyCentralImpulse(pullVector * _power);
    }
}
