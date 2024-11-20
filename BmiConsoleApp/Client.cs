/// <summary>
/// Represents a client with attributes including first name, last name, weight, and height.
/// </summary>
class Client
{
    private string _firstName;
    private string _lastName;
    private int _weight;
    private int _height;


    /// <summary>
    /// First name of the client.
    /// </summary>
    public string FirstName
    {
        get
        {
            return _firstName;
        }

        set
        {
            if (string.IsNullOrWhiteSpace(value)) 
            {
                throw new Exception("First Name field cannot be empty.");
            }

            _firstName = value.Trim();
        }
    }

    /// <summary>
    /// Last name of the client.
    /// </summary>
    public string LastName
    {
        get
        {
            return _lastName;
        }

        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("Last Name field cannot be empty.");
            }

            _lastName = value.Trim();
        }
    }

    /// <summary>
    /// Height of the client.
    /// </summary>
    public int Height
    {
        get
        {
            return _height;
        }

        set
        {
            if (value < 0)
            {
                throw new Exception("Height field cannot be less than 0.");
            }

            _height = value;
        }
    }

    /// <summary>
    /// Weight of the client.
    /// </summary>
    public int Weight
    {
        get
        {
            return _weight;
        }

        set
        {
            if (value < 0)
            {
                throw new Exception("Weight field cannot be less than 0.");
            }

            _weight = value;
        }
    }

    /// <summary>
    /// Creates a new client with the provided first name, last name, and weight.
    /// </summary>
    /// <param name="firstName">First name of the client.</param>
    /// <param name="lastName">Last name of the client.</param>
    /// <param name="weight">Weight of the client.</param>
    public Client(string firstName, string lastName, int weight, int height)
    {
        FirstName = firstName;
        LastName = lastName;
        Weight = weight;
        Height = height;
    }

    /// <summary>
    /// Displays the full name in Last Name, First Name format.
    /// </summary>
    public string FullName
    {
        get
        {
            return $"{LastName}, {FirstName}";
        }
    }

    /// <summary>
    /// Calculates and displays the BMI Score based on the weight and height.
    /// </summary>
    public double BmiScore
    {
        get
        {
            double bmiScore;
            bmiScore = (Weight / Math.Pow(Height, 2)) * 703;
            return bmiScore;
        }
    }

    /// <summary>
    /// Displays the BMI Status based on the BMI Score.
    /// </summary>
    public string BmiStatus
    {
        get
        {
            string bmiStatus;
            double bmiScore = BmiScore;

            if (bmiScore <= 18.4)
            {
                bmiStatus = "Underweight";
            }
            else if (bmiScore < 25.0)
            {
                bmiStatus = "Normal";
            }
            else if (bmiScore < 40.0)
            {
                bmiStatus = "Overweight";
            }
            else
            {
                bmiStatus = "Obese";
            }

            return bmiStatus;
        }
    }
}



